using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Enumerators;
using TesTool.Core.Models.Metadata;
using TesTool.Infra.Extensions;

namespace TesTool.Infra.Services
{
    public abstract class ProjectScanInfraServiceBase
    {
        private readonly ProjectType _projectType;
        private readonly ILoggerInfraService _loggerInfraService;

        protected ProjectScanInfraServiceBase(
            ProjectType projectType,
            ILoggerInfraService loggerInfraService
        )
        {
            _projectType = projectType;
            _loggerInfraService = loggerInfraService;
        }

        protected abstract string GetProjectFile();

        public async Task<bool> ProjectExistAsync()
        {
            var projectFile = GetProjectFile();
            if (string.IsNullOrWhiteSpace(projectFile)) return false;
            return await Task.FromResult(GetProject() is not null);
        }

        protected async Task<ITypeSymbol> GetTypeSymbolAsync(string name)
        {
            var project = GetProject();
            if (project is null) return default;

            var typeSymbol = null as ITypeSymbol;
            await ForEachClassesAsync((@class, root, model) => {
                if (typeSymbol is not null) return;

                var declaredSymbol = model.GetDeclaredSymbol(@class) as ITypeSymbol;
                if (declaredSymbol.GetName() == name) typeSymbol = declaredSymbol;
            });

            return typeSymbol;
        }

        protected async Task ForEachClassesAsync(
            Action<
                ClassDeclarationSyntax, 
                SyntaxNode, 
                SemanticModel
            > iterator
        ) 
        {
            var project = GetProject();
            if (project is null) throw new ProjectNotFoundException(_projectType);

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
        private readonly IDictionary<int, Class> _cacheDtos = new Dictionary<int, Class>();
        protected TypeWrapper GetModelType(ITypeSymbol typeSymbol)
        {
            var hash = (typeSymbol as dynamic).GetHashCode();
            if (_stackCalls.Contains(hash)) return default;

            if (typeSymbol.IsNullable())
            {
                _stackCalls.Push(hash);
                var modelType = GetModelType(typeSymbol.NullableOf());
                _stackCalls.Pop();

                return new Core.Models.Metadata.Nullable(modelType);
            }
            
            var name = typeSymbol.GetName();
            var @namespace = typeSymbol.GetNamespace();

            if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                var values = typeSymbol.GetMembers()
                    .OfType<IFieldSymbol>()
                    .Select(f => new { f.Name, Value = Convert.ToInt32(f.ConstantValue) })
                    .ToDictionary(k => k.Name, v => v.Value);

                return new Core.Models.Metadata.Enum(name, @namespace, values);
            }

            var systemType = Type.GetType(typeSymbol.GetFullMetadataName());
            var enumerableInnerType = typeSymbol.EnumerableOf();
            if (enumerableInnerType is not null && typeof(string) != systemType)
            {
                _stackCalls.Push(hash);
                var modelType = GetModelType(enumerableInnerType);
                _stackCalls.Pop();

                return new Core.Models.Metadata.Array(modelType);
            }

            if (typeSymbol.TypeKind == TypeKind.Interface) return default;
            if (systemType is not null) return new Field(systemType.Name, systemType.Namespace);

            if (_cacheDtos.ContainsKey(hash)) return _cacheDtos[hash];

            var @class = new Class(name, @namespace);
            var classMembers = typeSymbol.GetStackTypes().Reverse()
                .SelectMany(t => t.GetMembers())
                .Where(x => x.DeclaredAccessibility == Accessibility.Public);

            var usings = typeSymbol.ContainingNamespace.GetNamespaceTypes();
            foreach (var @using in usings) @class.AddNamespace(@using.ToString());

            var propertySymbols = classMembers.OfType<IPropertySymbol>().ToList();
            foreach (var propertySymbol in propertySymbols)
            {
                _stackCalls.Push(hash);
                var modelType = GetModelType(propertySymbol.Type);
                _stackCalls.Pop();

                var property = new Property(propertySymbol.Name, modelType);
                @class.AddProperty(property);
            }

            var methodSymbols = classMembers
                .OfType<IMethodSymbol>()
                .Where(m => m.MethodKind == MethodKind.Ordinary)
                .ToList();
            foreach (var methodSymbol in methodSymbols)
            {
                var method = new Method(methodSymbol.Name);
                @class.AddMethod(method);
            }

            _cacheDtos.Add(hash, @class);
            return @class;
        }

        private readonly IDictionary<string, Project> _cacheProjects = new Dictionary<string, Project>();
        protected Project GetProject()
        {
            var path = GetProjectFile();
            if (string.IsNullOrWhiteSpace(path)) return default;
            if (_cacheProjects.ContainsKey(path)) return _cacheProjects[path];

            var manager = new AnalyzerManager();
            var analyzer = manager.GetProject(path);
            var workspace = analyzer.GetWorkspace();
            var project = workspace.CurrentSolution.Projects.FirstOrDefault();
            _cacheProjects.Add(path, project);
            return project;
        }

        private readonly IDictionary<string, Compilation> _cacheCompilation = new Dictionary<string, Compilation>();
        protected async Task<Compilation> GetCompilationAsync(Project project)
        {
            if (_cacheCompilation.ContainsKey(project.Name)) return _cacheCompilation[project.Name];
            var compilation = await project.GetCompilationAsync();
            _cacheCompilation.Add(project.Name, compilation);
            return compilation;
        }

        public string GetNamespace() => GetProject()?.AssemblyName;
    }
}
