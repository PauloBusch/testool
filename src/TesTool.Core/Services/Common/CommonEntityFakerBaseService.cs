using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Common;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Common;

namespace TesTool.Core.Services.Common
{
    public class CommonEntityFakerBaseService : CommonBase, ICommonEntityFakerBaseService
    {
        public CommonEntityFakerBaseService(
            ISolutionInfraService solutionInfraService,
            ITestScanInfraService testScanInfraService
        ) : base(
            "Common.Abstract", HelpClassEnumerator.ENTITY_FAKER_BASE,
            solutionInfraService, testScanInfraService
        )
        { }

        public EntityFakerBase GetEntityFakerBaseModel(Class dbContextClass)
        {
            var entityFakerBaseModel = new EntityFakerBase(
                HelpClassEnumerator.ENTITY_FAKER_BASE.Name,
                GetNamespace(),
                dbContextClass
            );
            return entityFakerBaseModel;
        }
    }
}
