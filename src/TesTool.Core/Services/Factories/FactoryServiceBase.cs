using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Services.Factories
{
    public abstract class FactoryServiceBase
    {
        private readonly TestClass _testClass;

        protected readonly ISolutionService _solutionService;
        protected readonly ITestScanInfraService _testScanInfraService;
        protected readonly IWebApiScanInfraService _webApiScanInfraService;

        protected FactoryServiceBase(
            TestClass testClass,
            ISolutionService solutionService,
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
