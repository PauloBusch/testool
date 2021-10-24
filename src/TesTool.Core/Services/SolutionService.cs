using System.Linq;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Services
{
    public class SolutionService : ISolutionService
    {
        private readonly ISettingInfraService _settingInfraService;
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;

        public SolutionService(
            ISettingInfraService settingInfraService,
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        )
        {
            _settingInfraService = settingInfraService;
            _testScanInfraService = testScanInfraService;
            _webApiScanInfraService = webApiScanInfraService;
        }

        public string GetSolutionName()
        {
            var @namespace = _webApiScanInfraService.GetNamespace();
            if (string.IsNullOrWhiteSpace(@namespace)) return string.Empty;
            return @namespace.Split(".").FirstOrDefault();
        }

        public string GetTestFixtureClassName()
        {
            var fixtureName = _settingInfraService.GetStringAsync(SettingEnumerator.FIXTURE_NAME).Result;
            if (string.IsNullOrWhiteSpace(fixtureName)) 
                return TestClassEnumerator.FIXTURE.Name.Replace("{PROJECT_NAME}", GetSolutionName());
            return fixtureName;
        }

        public string GetTestNamespace(string sufix = null)
        {
            var integrationTestNamespace = _testScanInfraService.GetNamespace();
            if (!string.IsNullOrWhiteSpace(integrationTestNamespace)) return $"{integrationTestNamespace}.{sufix}";

            var webApiNamespace = _webApiScanInfraService.GetNamespace();
            return $"{webApiNamespace}.IntegrationTests.{sufix}";
        }
    }
}
