using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Services
{
    public class SolutionService : ISolutionService
    {
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;

        public SolutionService(
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        )
        {
            _testScanInfraService = testScanInfraService;
            _webApiScanInfraService = webApiScanInfraService;
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
