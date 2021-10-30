using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Common;

namespace TesTool.Core.Services.Common
{
    public class CommonRequestService : CommonBase, ICommonRequestService
    {
        public CommonRequestService(
            ISolutionInfraService solutionInfraService, 
            ITestScanInfraService testScanInfraService
        ) : base(
            "Common.Utils", HelpClassEnumerator.REQUEST, 
            solutionInfraService, testScanInfraService
        )
        { }
    }
}
