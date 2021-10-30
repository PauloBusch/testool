using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Common;

namespace TesTool.Core.Services.Common
{
    public class CommonProjectExplorerService : CommonBase, ICommonProjectExplorerService
    {
        public CommonProjectExplorerService(
            ISolutionInfraService solutionInfraService,
            ITestScanInfraService testScanInfraService
        ) : base(
            "Common.Utils", HelpClassEnumerator.PROJECT_EXPLORER,
            solutionInfraService, testScanInfraService
        )
        { }
    }
}
