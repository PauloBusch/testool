using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Models.Templates.Factory;

namespace TesTool.Core.Services.Factories
{
    public class FactoryCompareService : FactoryServiceBase, IFactoryCompareService
    {
        public FactoryCompareService(
            ISettingInfraService settingInfraService,
            ISolutionService solutionService,
            ITestScanInfraService testScanInfraService,
            IWebApiScanInfraService webApiScanInfraService
        ) : base(
            SettingEnumerator.COMPARATOR_FACTORY_NAME,
            TestClassEnumerator.COMPARE_FACTORY,
            settingInfraService, solutionService,
            testScanInfraService, webApiScanInfraService
        )
        { }

        public ComparatorFactory GetModelFactory(string name)
        {
            return new ComparatorFactory(name, GetNamespace(), default);
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestNamespace("Assertions");
        }
    }
}
