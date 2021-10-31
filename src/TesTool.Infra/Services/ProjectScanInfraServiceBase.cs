using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Enumerators;
using TesTool.Core.Models.Metadata;
using TesTool.Infra.Extensions;
using TesTool.Infra.Models;

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

        public abstract string GetProjectPathFile();

        public async Task<bool> ProjectExistAsync()
        {
            var projectFile = GetProjectPathFile();
            if (string.IsNullOrWhiteSpace(projectFile)) return false;
            return await GetProjectAsync() is not null;
        }

        protected async Task<IEnumerable<ClassContext>> GetClassesAsync(IEnumerable<Project> projects)
        {
            var classes = new List<ClassContext>();
            if (projects is null || !projects.Any()) return classes;

            foreach (var project in projects) classes.AddRange(await GetClassesAsync(project));
            return classes;
        }

        protected async Task<IEnumerable<ClassContext>> GetClassesAsync()
        {
            var project = await GetProjectAsync();
            if (project is null) return new List<ClassContext>();

            return await GetClassesAsync(project);
        }

        private static List<ProjectContext> _cacheProjectContext = new List<ProjectContext>();
        private async Task<IEnumerable<ClassContext>> GetClassesAsync(Project project)
        {
            var existProjectContext = _cacheProjectContext.FirstOrDefault(p => p.Id == project.Id);
            if (existProjectContext is not null) return existProjectContext.Classes;

            var projectContext = new ProjectContext(project.Id);
            var compilation = await GetCompilationAsync(project);
            foreach (var document in project.Documents)
            {
                var root = await document.GetSyntaxRootAsync();
                var tree = await document.GetSyntaxTreeAsync();
                var model = compilation.GetSemanticModel(tree);
                var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (var @class in classes)
                {
                    var typeSymbol = model.GetDeclaredSymbol(@class) as ITypeSymbol;
                    var classContext = new ClassContext(@class, typeSymbol, root, document.FilePath);
                    projectContext.Classes.Add(classContext);
                }
            }

            _cacheProjectContext.Add(projectContext);
            return projectContext.Classes;
        }

        private static Stack<int> _stackCalls = new Stack<int>();
        private static IDictionary<int, Class> _cacheDtos = new Dictionary<int, Class>();
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

                var array = new Core.Models.Metadata.Array(name, @namespace, modelType);
                var arrayGenerics = typeSymbol.GetGeneritTypeArguments();
                if (arrayGenerics is null) return array;

                foreach (var generic in arrayGenerics)
                {
                    _stackCalls.Push(hash);
                    var genericType = GetModelType(generic);
                    _stackCalls.Pop();

                    if (genericType is not null) array.AddGeneric(genericType);
                }

                return array;
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

            var generics = typeSymbol.GetGeneritTypeArguments();
            foreach (var generic in generics)
            {
                _stackCalls.Push(hash);
                var genericType = GetModelType(generic);
                _stackCalls.Pop();

                if (genericType is not null) @class.AddGeneric(genericType);
            }

            var propertySymbols = classMembers.OfType<IPropertySymbol>().ToList();
            foreach (var propertySymbol in propertySymbols)
            {
                _stackCalls.Push(hash);
                var modelType = GetModelType(propertySymbol.Type);
                _stackCalls.Pop();

                var fromGeneric = generics.Any(g => g.GetFullMetadataName() == propertySymbol.Type.GetFullMetadataName());
                var property = new Property(propertySymbol.Name, fromGeneric, modelType);
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

        private static readonly List<Project> _cacheProjects = new List<Project>();
        protected async Task<Project> GetProjectAsync()
        {
            var projectPathFile = GetProjectPathFile();
            if (string.IsNullOrWhiteSpace(projectPathFile)) return default;
            
            var projectName = Path.GetFileNameWithoutExtension(projectPathFile);
            var existingProject = _cacheProjects.FirstOrDefault(p => p.Name == projectName);
            if (existingProject is not null) return existingProject;

            if (MSBuildLocator.CanRegister) MSBuildLocator.RegisterDefaults();
            using var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(projectPathFile);
            _cacheProjects.Add(project);
            return project;
        }

        protected IEnumerable<Project> GetProjectReferences(Project project)
        {
            var projects = new List<Project>();
            foreach (var projectReference in project.ProjectReferences)
            {
                var existingProject = _cacheProjects.FirstOrDefault(p => p.Id == projectReference.ProjectId);
                if (existingProject is not null)
                {
                    projects.Add(existingProject);
                    continue;
                }

                var projectChild = project.Solution.GetProject(projectReference.ProjectId);
                _cacheProjects.Add(projectChild);
                projects.Add(projectChild);
            }
            return projects;
        }

        private static readonly IDictionary<ProjectId, Compilation> _cacheCompilation = new Dictionary<ProjectId, Compilation>();
        protected async Task<Compilation> GetCompilationAsync(Project project)
        {
            if (_cacheCompilation.ContainsKey(project.Id)) return _cacheCompilation[project.Id];
            var compilation = await project.GetCompilationAsync();
            _cacheCompilation.Add(project.Id, compilation);
            return compilation;
        }

        public void ClearCache()
        {
            var projectPathFile = GetProjectPathFile();
            if (string.IsNullOrWhiteSpace(projectPathFile)) return;

            var projectName = Path.GetFileNameWithoutExtension(projectPathFile);
            var existingProject = _cacheProjects.FirstOrDefault(p => p.Name == projectName);
            if (existingProject is null) return;

            _cacheProjects.Remove(existingProject); 
            _cacheCompilation.Remove(existingProject.Id);
            _cacheProjectContext.RemoveAll(c => c.Id == existingProject.Id);
        }

        public string GetName()
        {
            return GetProjectAsync().Result?.Name;
        }

        public string GetNamespace() => GetProjectAsync().Result?.AssemblyName;
    }
}
