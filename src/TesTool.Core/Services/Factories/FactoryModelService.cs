using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Models.Templates.Factory;

namespace TesTool.Core.Services.Factories
{
    public class FactoryModelService : FactoryServiceBase, IFactoryModelService
    {
        public FactoryModelService(
            ITestScanInfraService testScanInfraService, 
            IWebApiScanInfraService webApiScanInfraService
        ) : base(testScanInfraService, webApiScanInfraService) { }

        public ModelFactory GetModelFactory(string name)
        {
            return new ModelFactory(name, GetNamespace("Fakers"));
        }
    }
}
