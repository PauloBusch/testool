using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.Infra.Extensions;

namespace TesTool.Infra.Services
{
    public abstract class ProjectScanInfraServiceBase
    {
        private readonly ILoggerInfraService _loggerInfraService;

        protected ProjectScanInfraServiceBase(ILoggerInfraService loggerInfraService)
        {
            _loggerInfraService = loggerInfraService;
        }

        protected abstract string GetProjectPath();

        protected async Task ForEachClassesAsync(
            Action<
                ClassDeclarationSyntax, 
                SyntaxNode, 
                SemanticModel
            > iterator
        ) 
        {
            var project = GetProject();
            if (project is null)
            {
                _loggerInfraService.LogError("No projects found.");
                return;
            }

            var compilation = await GetCompilationAsync(project);
            foreach (var documentId in project.DocumentIds)
            {
                var document = project.GetDocument(documentId);
                var root = await document.GetSyntaxRootAsync();
                var tree = await document.GetSyntaxTreeAsync();
                var model = compilation.GetSemanticModel(tree);
                var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (var @class in classes) iterator(@class, root, model);
            }
        }

        private readonly Stack<int> _stackCalls = new Stack<int>();
        private readonly IDictionary<int, Dto> _cacheDtos = new Dictionary<int, Dto>();
        protected TypeBase GetModelType(ITypeSymbol typeSymbol)
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

            var propertySymbols = typeSymbol.GetStackTypes().SelectMany(t => t.GetMembers())
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

            _cacheDtos.Add(hash, dto);
            return dto;
        }

        private readonly IDictionary<string, Project> _cacheProjects = new Dictionary<string, Project>();
        private Project GetProject()
        {
            var path = GetProjectPath();
            if (_cacheProjects.ContainsKey(path)) return _cacheProjects[path];

            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(path);
            var workspace = analyzer.GetWorkspace();
            var project = workspace.CurrentSolution.Projects.FirstOrDefault();
            _cacheProjects.Add(path, project);
            return project;
        }

        private readonly IDictionary<string, Compilation> _cacheCompilation = new Dictionary<string, Compilation>();
        private async Task<Compilation> GetCompilationAsync(Project project)
        {
            if (_cacheCompilation.ContainsKey(project.Name)) return _cacheCompilation[project.Name];
            var compilation = await project.GetCompilationAsync();
            _cacheCompilation.Add(project.Name, compilation);
            return compilation;
        }
    }
}
