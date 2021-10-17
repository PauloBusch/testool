using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Models.Templates.Factory;

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
            SettingEnumerator.MODEL_FACTORY_NAME,
            TestClassEnumerator.MODEL_FACTORY,
            settingInfraService, solutionService, 
            testScanInfraService, webApiScanInfraService
        ) { }

        public ModelFactory GetModelFactory(string name)
        {
            return new ModelFactory(name, GetNamespace());
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestNamespace("Fakers");
        }
    }
}
