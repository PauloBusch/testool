using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.Infra.Extensions;

namespace TesTool.Infra.Services
{
    public class TestScanInfraService : ProjectScanInfraServiceBase, ITestScanInfraService
    {
        private readonly IEnvironmentInfraService _environmentInfraService;

        public TestScanInfraService(
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

        public async Task<IEnumerable<string>> GetNotFoundClassesAsync(IEnumerable<string> regexNames)
        {
            var project = GetProject();
            if (project is null) return default;

            var notFounded = regexNames.ToList();
            await ForEachClassesAsync((@class, root, model) => {
                if (!notFounded.Any()) return true;

                var declaredSymbol = model.GetDeclaredSymbol(@class) as ITypeSymbol;
                notFounded.RemoveAll(regex => Regex.IsMatch(declaredSymbol.GetName(), regex));
                return notFounded.Any();
            });

            return notFounded;
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
