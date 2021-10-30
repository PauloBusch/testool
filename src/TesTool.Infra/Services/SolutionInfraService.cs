using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class SolutionInfraService : ISolutionInfraService
    {
        private readonly ISettingInfraService _settingInfraService;
        private readonly IEnvironmentInfraService _environmentInfraService;
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;

        public SolutionInfraService(
            ISettingInfraService settingInfraService, 
            IEnvironmentInfraService environmentInfraService, 
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        )
        {
            _settingInfraService = settingInfraService;
            _environmentInfraService = environmentInfraService;
            _testScanInfraService = testScanInfraService;
            _webApiScanInfraService = webApiScanInfraService;
        }

        public string GetSolutionName()
        {
            var solutionName = Path.GetFileName(GetSolutionFilePath());
            return Regex.Replace(solutionName, @"\W", string.Empty);
        }

        public string GetTestFixtureClassName()
        {
            var fixtureName = _settingInfraService.GetStringAsync(SettingEnumerator.FIXTURE_NAME).Result;
            if (string.IsNullOrWhiteSpace(fixtureName))
                return HelpClassEnumerator.FIXTURE.Name.Replace("{PROJECT_NAME}", GetSolutionName());
            return fixtureName;
        }

        public string GetTestName()
        {
            var integrationTestName = _testScanInfraService.GetName();
            return string.IsNullOrWhiteSpace(integrationTestName)
                ? $"{_webApiScanInfraService.GetName()}.IntegrationTests"
                : integrationTestName;
        }

        public string GetTestNamespace(string sufix = null)
        {
            var integrationTestNamespace = _testScanInfraService.GetNamespace();
            var @namespace = string.IsNullOrWhiteSpace(integrationTestNamespace)
                ? $"{_webApiScanInfraService.GetNamespace()}.IntegrationTests"
                : integrationTestNamespace;

            return !string.IsNullOrWhiteSpace(sufix) ? $"{@namespace}.{sufix}" : @namespace;
        }

        private static string _cacheSolutionFilePath;
        public string GetSolutionFilePath()
        {
            if (!string.IsNullOrWhiteSpace(_cacheSolutionFilePath)) return _cacheSolutionFilePath;

            var webApiProjectPathFile = _settingInfraService.GetStringAsync(SettingEnumerator.PROJECT_DIRECTORY).Result;
            var applicationBasePath = string.IsNullOrWhiteSpace(webApiProjectPathFile) || !Directory.Exists(Path.GetDirectoryName(webApiProjectPathFile))
                ? _environmentInfraService.GetWorkingDirectory() : Path.GetDirectoryName(webApiProjectPathFile);
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var projectDirectoryInfo = new DirectoryInfo(directoryInfo.FullName);
                if (projectDirectoryInfo.Exists)
                {
                    var projectFiles = Directory.GetFiles(projectDirectoryInfo.FullName, "*.sln");
                    if (projectFiles.Any())
                    {
                        _cacheSolutionFilePath = projectFiles.First();
                        return _cacheSolutionFilePath;
                    }
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            throw new ValidationException("Solução não encontrada.");
        }
    }
}
