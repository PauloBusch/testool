using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Services.Factories
{
    public class FactoryCompareService : FactoryServiceBase, IFactoryCompareService
    {
        public FactoryCompareService(
            ISolutionService solutionService,
            ITestScanInfraService testScanInfraService,
            IWebApiScanInfraService webApiScanInfraService
        ) : base(
            HelpClassEnumerator.COMPARE_FACTORY,
            solutionService, testScanInfraService, 
            webApiScanInfraService
        )
        { }

        public string GetDirectoryBase()
        {
            return $"{_testScanInfraService.GetDirectoryBase()}/Assertions";
        }

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
