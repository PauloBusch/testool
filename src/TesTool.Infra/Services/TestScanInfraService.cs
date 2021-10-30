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
        protected readonly ISettingInfraService _settingInfraService;
        protected readonly IEnvironmentInfraService _environmentInfraService;

        public TestScanInfraService(
            ILoggerInfraService loggerInfraService,
            ISettingInfraService settingInfraService,
            IEnvironmentInfraService environmentInfraService
        ) : base(ProjectTypeEnumerator.INTEGRATION_TESTS, loggerInfraService) 
        { 
            _settingInfraService = settingInfraService;
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

        public string GetDirectoryBase()
        {
            var projectPathFile = GetProjectPathFile();
            return Path.GetDirectoryName(projectPathFile);
        }

        private static string _cacheProjectPath;
        public override string GetProjectPathFile()
        {
            if (!string.IsNullOrWhiteSpace(_cacheProjectPath)) return _cacheProjectPath;

            var settingProjectPathFile = _settingInfraService.GetStringAsync(SettingEnumerator.PROJECT_INTEGRATION_TEST_DIRECTORY).Result;
            if (!string.IsNullOrWhiteSpace(settingProjectPathFile) && IsTestProjectFile(settingProjectPathFile)) return settingProjectPathFile;

            var applicationBasePath = _environmentInfraService.GetWorkingDirectory();
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var projectDirectoryInfo = new DirectoryInfo(directoryInfo.FullName);
                if (projectDirectoryInfo.Exists)
                {
                    var projectFiles = Directory.GetFiles(projectDirectoryInfo.FullName, "*.csproj");
                    foreach (var projectPathFile in projectFiles)
                    {
                        if (IsTestProjectFile(projectPathFile))
                        {
                            _cacheProjectPath = projectPathFile;
                            return _cacheProjectPath;
                        }
                    }
                }
                
                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            return default;
        }

        private bool IsTestProjectFile(string projectPathFile)
        {
            if (!File.Exists(projectPathFile)) return false;
            var packages = GetProjectPackages(projectPathFile);
            return packages.Any(p => p.Include == "Microsoft.NET.Test.Sdk");
        }
    }
}
