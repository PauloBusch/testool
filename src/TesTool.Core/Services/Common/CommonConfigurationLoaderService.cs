using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Common;

namespace TesTool.Core.Services.Common
{
    public class CommonConfigurationLoaderService : CommonBase, ICommonConfigurationLoaderService
    {
        public CommonConfigurationLoaderService(
            ISolutionInfraService solutionInfraService,
            ITestScanInfraService testScanInfraService
        ) : base(
            "Common.Utils", HelpClassEnumerator.CONFIGURATION_LOADER,
            solutionInfraService, testScanInfraService
        )
        { }
    }
}
