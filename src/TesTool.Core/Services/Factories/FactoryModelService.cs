using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Services.Factories
{
    public class FactoryModelService : FactoryServiceBase, IFactoryModelService
    {
        public FactoryModelService(
            ISolutionService solutionService,
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        ) : base(
            TestClassEnumerator.MODEL_FAKER_FACTORY,
            solutionService, testScanInfraService, webApiScanInfraService
        ) { }

        public ModelFakerFactory GetModelFactory(string name)
        {
            return new ModelFakerFactory(name, GetNamespace());
        }
        public string GetDirectoryBase()
        {
            return $"{_testScanInfraService.GetDirectoryBase()}/Fakers";
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestNamespace("Fakers");
        }
    }
}
