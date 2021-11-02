using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Common;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Common;

namespace TesTool.Core.Services.Common
{
    public class CommonTestBaseService : CommonBase, ICommonTestBaseService
    {
        private readonly IFixtureService _fixtureService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFactoryModelService _factoryModelService;
        private readonly IFactoryEntityService _factoryEntityService;
        private readonly IFactoryCompareService _factoryCompareService;

        public CommonTestBaseService(
            IFixtureService fixtureService,
            ICommonRequestService commonRequestService,
            IFactoryModelService factoryModelService,
            IFactoryEntityService factoryEntityService,
            IFactoryCompareService factoryCompareService,
            ISolutionInfraService solutionInfraService,
            ITestScanInfraService testScanInfraService
        ) : base(
            "Common.Abstract", HelpClassEnumerator.TEST_BASE,
            solutionInfraService, testScanInfraService
        )
        { 
            _fixtureService = fixtureService;
            _commonRequestService = commonRequestService;
            _factoryModelService = factoryModelService;
            _factoryEntityService = factoryEntityService;
            _factoryCompareService = factoryCompareService;
        }

        public TestBase GetTestBaseModel(Class dbContextClass, bool auth)
        {
            var testBaseModel = new TestBase(
                auth, _helpClass.Name, GetNamespace(),
                _fixtureService.GetFixtureName(),
                dbContextClass
            );
            testBaseModel.AddNamespace(_commonRequestService.GetNamespace());
            testBaseModel.AddNamespace(_factoryModelService.GetNamespace());
            testBaseModel.AddNamespace(_factoryEntityService.GetNamespace());
            testBaseModel.AddNamespace(_factoryCompareService.GetNamespace());
            return testBaseModel;
        }
    }
}
