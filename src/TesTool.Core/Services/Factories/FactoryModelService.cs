using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Services.Factories
{
    public class FactoryModelService : FactoryServiceBase, IFactoryModelService
    {
        public FactoryModelService(
            ISettingInfraService settingInfraService,
            ISolutionService solutionService,
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        ) : base(
            SettingEnumerator.MODEL_FAKER_FACTORY_NAME,
            TestClassEnumerator.MODEL_FAKER_FACTORY,
            settingInfraService, solutionService, 
            testScanInfraService, webApiScanInfraService
        ) { }

        public ModelFakerFactory GetModelFactory(string name)
        {
            return new ModelFakerFactory(name, GetNamespace());
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestNamespace("Fakers");
        }
    }
}
