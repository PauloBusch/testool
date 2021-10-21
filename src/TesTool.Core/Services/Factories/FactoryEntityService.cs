using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Services.Factories
{
    public class FactoryEntityService : FactoryServiceBase, IFactoryEntityService
    {
        public FactoryEntityService(
            ISettingInfraService settingInfraService, 
            ISolutionService solutionService, 
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        ) : base(
            SettingEnumerator.ENTITY_FAKER_FACTORY_NAME,
            TestClassEnumerator.ENTITY_FAKER_FACTORY,
            settingInfraService, solutionService,
            testScanInfraService, webApiScanInfraService
        ) { }

        public async Task<EntityFakerFactory> GetEntityFactoryAsync(string name, string dbContext)
        {
            var dbContextClass = await _webApiScanInfraService.GetModelAsync(dbContext) as Class;
            var testBaseClass = await _testScanInfraService.GetClassAsync(TestClassEnumerator.TEST_BASE.Name);
            return new EntityFakerFactory(name, GetNamespace(), testBaseClass, dbContextClass);
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestNamespace("Fakers");
        }
    }
}
