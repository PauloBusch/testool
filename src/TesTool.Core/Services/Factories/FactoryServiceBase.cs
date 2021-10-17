using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Configuration;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Services.Factories
{
    public abstract class FactoryServiceBase
    {
        private readonly Setting _setting;
        private readonly TestClass _testClass;

        private readonly ISettingInfraService _settingInfraService;
        
        protected readonly ISolutionService _solutionService;
        protected readonly ITestScanInfraService _testScanInfraService;
        protected readonly IWebApiScanInfraService _webApiScanInfraService;

        protected FactoryServiceBase(
            Setting setting,
            TestClass testClass,
            ISettingInfraService settingInfraService,
            ISolutionService solutionService,
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        )
        {
            _setting = setting;
            _testClass = testClass;
            _solutionService = solutionService;
            _settingInfraService = settingInfraService;
            _testScanInfraService = testScanInfraService;
            _webApiScanInfraService = webApiScanInfraService;
        }

        public async Task<string> GetFactoryNameAsync()
        {
            var factoryName = await _settingInfraService.GetStringAsync(_setting.Key);
            return !string.IsNullOrWhiteSpace(factoryName) ? factoryName : _testClass.Name;
        }
    }
}
