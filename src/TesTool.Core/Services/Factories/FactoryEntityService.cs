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
            ISolutionService solutionService, 
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        ) : base(
            TestClassEnumerator.ENTITY_FAKER_FACTORY,
            solutionService, testScanInfraService, webApiScanInfraService
        ) { }

        public string GetDirectoryBase()
        {
            return $"{_testScanInfraService.GetDirectoryBase()}/Fakers";
        }

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
