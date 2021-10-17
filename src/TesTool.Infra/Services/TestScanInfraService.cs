using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;

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
            var classes = await GetClassesAsync();
            return classes.Any(c => c.Declaration.Identifier.Text == className);
        }

        public async Task<Class> GetClassAsync(string className)
        {
            var classes = await GetClassesAsync();
            var @class = classes.FirstOrDefault(c => c.Declaration.Identifier.Text == className);
            if (@class is null) return default;
            
            return GetModelType(@class.TypeSymbol) as Class;
        }

        public async Task<string> GetPathClassAsync(string className)
        {
            var classes = await GetClassesAsync();
            return classes.FirstOrDefault(c => c.Declaration.Identifier.Text == className)?.FilePath;
        }

        private static string _cacheProjectPath;
        protected override string GetProjectPathFile()
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
