using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Services.Factories
{
    public abstract class FactoryServiceBase
    {
        private readonly HelpClass _testClass;

        protected readonly ISolutionInfraService _solutionService;
        protected readonly ITestScanInfraService _testScanInfraService;
        protected readonly IWebApiScanInfraService _webApiScanInfraService;

        protected FactoryServiceBase(
            HelpClass testClass,
            ISolutionInfraService solutionService,
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        )
        {
            _testClass = testClass;
            _solutionService = solutionService;
            _testScanInfraService = testScanInfraService;
            _webApiScanInfraService = webApiScanInfraService;
        }

        public string GetFactoryName()
        {
            return _testClass.Name;
        }
    }
}
