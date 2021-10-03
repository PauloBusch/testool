using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.Infra.Extensions;

namespace TesTool.Infra.Services
{
    public class ProjectScanInfraService : IProjectScanInfraService
    {
        private readonly ILoggerInfraService _loggerInfraService;
        private readonly ISettingInfraService _settingInfraService;

        public ProjectScanInfraService(
            ILoggerInfraService loggerInfraService,
            ISettingInfraService settingInfraService
        )
        {
            _loggerInfraService = loggerInfraService;
            _settingInfraService = settingInfraService;
        }

        public async Task<IEnumerable<Controller>> GetControllersAsync()
        {
            var controllers = new List<Controller>();
            var project = await GetCurrentProjectAsync();
            if (project is null)
            {
                _loggerInfraService.LogError("No projects found.");
                return controllers;
            }

            var compilation = await project.GetCompilationAsync();
            foreach (var documentId in project.DocumentIds)
            {
                var document = project.GetDocument(documentId);
                var root = await document.GetSyntaxRootAsync();
                var tree = await document.GetSyntaxTreeAsync();
                var model = compilation.GetSemanticModel(tree);
                var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (var @class in classes)
                {
                    if (@class.IsAbstract()) continue;

                    var classSymbol = model.GetDeclaredSymbol(@class) as ITypeSymbol;
                    var isController = classSymbol.ImplementsClass("ControllerBase", "Microsoft.AspNetCore.Mvc");
                    if (!isController) continue;

                    var controllerName = @class.Identifier.Text.ToLower().Replace("controller", string.Empty);
                    var routeAttribute = classSymbol.GetAttribute("RouteAttribute");
                    var routeTemplate = routeAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;
                    var route = routeTemplate.Replace("[controller]", controllerName);
                    var controller = new Controller(@class.Identifier.Text, route);
                    FillEndpoints(controller, root, model);
                    if (controller.Endpoints.Any()) controllers.Add(controller);
                }
            }

            return controllers;
        }

        private async Task<Project> GetCurrentProjectAsync()
        {
            var projectPath = await _settingInfraService.GetStringAsync(SettingEnumerator.PROJECT_DIRECTORY.Key);
            if (!File.Exists(projectPath)) return default;
            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(projectPath);
            var workspace = analyzer.GetWorkspace();
            return workspace.CurrentSolution.Projects.FirstOrDefault();
        }

        private void FillEndpoints(Controller controller, SyntaxNode root, SemanticModel model)
        {
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var method in methods)
            {
                var methodSymbol = model.GetDeclaredSymbol(method);
                var methodAttribute = methodSymbol.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass.Name.StartsWith("Http"));
                if (methodAttribute is null) continue;
                
                var methodType = MethodEnumerator.GetAll()
                    .FirstOrDefault(m => methodAttribute.AttributeClass.Name.Contains(m.Name));
                if (methodType is null) continue;

                var route = methodAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString();
                var endpoint = new Endpoint(route, methodType);
                FillInputs(endpoint, methodSymbol);
                FillOutput(endpoint, methodSymbol);
                controller.AddEndpoint(endpoint);
            }
        }

        private void FillInputs(Endpoint endpoint, IMethodSymbol methodSymbol)
        {
            foreach(var parameter in methodSymbol.Parameters)
            {
                var sourceAttribute = parameter.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass.Name.StartsWith("From"));
                var inputSource = sourceAttribute is null ? null : InputSourceEnumerator.GetAll()
                    .FirstOrDefault(m => sourceAttribute.AttributeClass.Name.Contains(m.Name));
                if (inputSource is null)
                {
                    var route = endpoint.Route ?? string.Empty;
                    inputSource = route.Contains($"{{{parameter.Name}}}") 
                        ? InputSourceEnumerator.ROUTE : InputSourceEnumerator.BODY;
                }

                endpoint.AddInput(new Input(parameter.Name, GetModelType(parameter.Type), inputSource));
            }
        }

        private void FillOutput(Endpoint endpoint, IMethodSymbol methodSymbol)
        {
            var asyncResult = methodSymbol.ReturnType.BaseType.Name == typeof(Task).Name;
            var returnType = asyncResult
                ? methodSymbol.ReturnType.GetGeneritTypeArguments().First()
                : methodSymbol.ReturnType;

            endpoint.Output = GetModelType(returnType);
        }

        private readonly Stack<int> _stackCalls = new Stack<int>();
        private readonly IDictionary<int, Dto> _cacheDtos = new Dictionary<int, Dto>();
        private TypeBase GetModelType(ITypeSymbol typeSymbol)
        {
            var hash = (typeSymbol as dynamic).GetHashCode();
            if (_stackCalls.Contains(hash)) return default;

            var definition = typeSymbol.GetFullName();
            if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                var values = typeSymbol.GetMembers()
                    .OfType<IFieldSymbol>()
                    .Select(f => new { f.Name, Value = Convert.ToInt32(f.ConstantValue) })
                    .ToDictionary(k => k.Name, v => v.Value);
                
                return new Core.Models.Metadata.Enum(definition, values);
            }

            var systemType = Type.GetType(typeSymbol.GetFullMetadataName());
            var enumerableInnerType = typeSymbol.EnumerableOf();
            if (enumerableInnerType is not null && typeof(string) != systemType)
            {
                _stackCalls.Push(hash);
                var modelType = GetModelType(enumerableInnerType);
                _stackCalls.Pop();

                return new Core.Models.Metadata.Array(definition, modelType);
            }

            if (typeSymbol.TypeKind == TypeKind.Interface) return default;
            if (systemType is not null) return new Field(definition, systemType.ToString());

            var propertySymbols = typeSymbol.GetTypes().SelectMany(t => t.GetMembers())
                .Where(x => x.DeclaredAccessibility == Accessibility.Public)
                .OfType<IPropertySymbol>()
                .ToList();

            if (_cacheDtos.ContainsKey(hash)) return _cacheDtos[hash];

            var dto = new Dto(definition);
            foreach (var propertySymbol in propertySymbols)
            {
                _stackCalls.Push(hash);
                var modelType = GetModelType(propertySymbol.Type);
                _stackCalls.Pop();

                var property = new Property(propertySymbol.Name, modelType);
                dto.AddProperty(property);
            }

            _cacheDtos[hash] = dto;
            return dto;
        }
    }
}
