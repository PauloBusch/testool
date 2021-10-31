using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Fakers;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Services.Fakers
{
    public class FakeModelService : FakeServiceBase, IFakeModelService
    {
        public FakeModelService(
            ISolutionInfraService solutionService,
            ITestScanInfraService testScanInfraService,
            IExpressionInfraService expressionInfraService,
            IConventionInfraService conventionInfraService
        ) : base(
            solutionService, testScanInfraService,
            expressionInfraService, conventionInfraService
        ) { }

        public string GetDirectoryBase()
        {
            return $"{_testScanInfraService.GetDirectoryBase()}/Fakers/Models";
        }

        public async Task<ModelFaker> GetFakerModelAsync(Class model, bool @static)
        {
            var templateModel = new ModelFaker(model.Name, GetNamespace());
            return await FillTemplateAsync<ModelFaker, ModelProperty>(templateModel, model, @static);
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestProjectNamespace("Fakers.Models");
        }

        protected override T MapProperty<T>(string name, string expression, bool @unsafe)
        {
            return new ModelProperty(name, expression, @unsafe) as T;
        }
    }
}
