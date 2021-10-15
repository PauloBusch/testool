using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;

namespace TesTool.Infra.Services
{
    public class IntegrationTestScanInfraService : ProjectScanInfraServiceBase, IIntegrationTestScanInfraService
    {
        private readonly IEnvironmentInfraService _environmentInfraService;

        public IntegrationTestScanInfraService(
            ILoggerInfraService loggerInfraService,
            IEnvironmentInfraService environmentInfraService
        ) : base(ProjectTypeEnumerator.INTEGRATION_TESTS, loggerInfraService) 
        { 
            _environmentInfraService = environmentInfraService;
        }

        public async Task<bool> ClassExistAsync(string className)
        {
            return await GetTypeSymbolAsync(className) is not null;
        }

        public async Task<Class> GetClassAsync(string name)
        {
            var typeSymbol = await GetTypeSymbolAsync(name);
            if (typeSymbol is null) return default;
            return GetModelType(typeSymbol) as Class;
        }

        public async Task<string> GetPathClassAsync(string className)
        {
            var project = GetProject();
            if (project is null) return default;

            var compilation = await GetCompilationAsync(project);
            foreach (var documentId in project.DocumentIds)
            {
                var document = project.GetDocument(documentId);
                var root = await document.GetSyntaxRootAsync();
                var tree = await document.GetSyntaxTreeAsync();
                var model = compilation.GetSemanticModel(tree);
                var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                if (classes.Any(c => c.Identifier.Text == className)) return document.FilePath;
            }

            return default;
        }

        public async Task<string> MergeClassCodeAsync(string className, string sourceCode)
        {
            var project = GetProject();
            if (project is null) return default;

            var compilationUnitGenerated = SyntaxFactory.ParseCompilationUnit(sourceCode);
            var sourceClass = compilationUnitGenerated.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Single(c => c.Identifier.Text == className);
            var sourceUsings = compilationUnitGenerated.Usings;
            var sourceMethods = sourceClass.Members.OfType<MethodDeclarationSyntax>().ToList();

            var mergedClassCode = null as string;
            await ForEachClassesAsync((storedClass, root, model) => {
                if (mergedClassCode is not null) return;
                if (storedClass.Identifier.Text != className) return;

                var updatedClass = null as ClassDeclarationSyntax;
                var compilationUnitStored = root as CompilationUnitSyntax;
                var storedUsings = compilationUnitStored.Usings;
                var storedMethods = storedClass.Members.OfType<MethodDeclarationSyntax>().ToList();

                var usingsToAppend = sourceUsings.Where(u => !storedUsings.Any(s => s.Name.ToString() == u.Name.ToString())).ToList();
                var methodsToAppend = sourceMethods.Where(s => !storedMethods.Any(m => m.Identifier.Text == s.Identifier.Text)).ToList();
                
                foreach (var @using in usingsToAppend) compilationUnitStored = compilationUnitStored.AddUsings(@using);
                foreach (var method in methodsToAppend) updatedClass = (updatedClass ?? storedClass).AddMembers(method);

                if (updatedClass is not null) compilationUnitStored = compilationUnitStored.ReplaceNode(storedClass, updatedClass);
                mergedClassCode = compilationUnitStored.ToFullString();
            });

            return mergedClassCode;
        }

        private string _cacheProjectPath;
        protected override string GetProjectFile()
        {
            if (!string.IsNullOrWhiteSpace(_cacheProjectPath)) return _cacheProjectPath;
            
            var applicationBasePath = _environmentInfraService.GetWorkingDirectory();
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var projectDirectoryInfo = new DirectoryInfo(directoryInfo.FullName);
                if (projectDirectoryInfo.Exists)
                {
                    var projectFiles = Directory.GetFiles(projectDirectoryInfo.FullName, "*.csproj");
                    if (projectFiles.Any()) {
                        _cacheProjectPath = projectFiles.First();
                        return _cacheProjectPath;
                    }
                }
                
                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            return default;
        }
    }
}
