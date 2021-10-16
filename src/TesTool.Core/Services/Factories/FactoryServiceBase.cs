using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Services.Factories
{
    public abstract class FactoryServiceBase
    {
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;

        protected FactoryServiceBase(
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        )
        {
            _testScanInfraService = testScanInfraService;
            _webApiScanInfraService = webApiScanInfraService;
        }

        protected string GetNamespace(string sufix)
        {
            var integrationTestNamespace = _testScanInfraService.GetNamespace();
            if (!string.IsNullOrWhiteSpace(integrationTestNamespace)) return $"{integrationTestNamespace}.{sufix}";

            var webApiNamespace = _webApiScanInfraService.GetNamespace();
            return $"{webApiNamespace}.IntegrationTests.{sufix}";
        }
    }
}
