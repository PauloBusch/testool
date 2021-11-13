using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Common;

namespace TesTool.Core.Services.Common
{
    public class CommonXUnitRunnerService : ICommonXUnitRunnerService
    {
        protected readonly ITestScanInfraService _testScanInfraService;

        public CommonXUnitRunnerService(ITestScanInfraService testScanInfraService)
        {
            _testScanInfraService = testScanInfraService;
        }

        public string GetPathFile()
        {
            return @$"{_testScanInfraService.GetDirectoryBase()}\xunit.runner.json";
        }
    }
}
