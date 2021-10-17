using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Services.Fakers
{
    public class FakeServiceBase
    {
        protected readonly ISolutionService _solutionService;
        protected readonly ITestScanInfraService _testScanInfraService;
        protected readonly IExpressionInfraService _expressionInfraService;

        public FakeServiceBase(
            ISolutionService solutionService,
            ITestScanInfraService testScanInfraService,
            IExpressionInfraService expressionInfraService
        )
        {
            _solutionService = solutionService;
            _testScanInfraService = testScanInfraService;
            _expressionInfraService = expressionInfraService;
        }
    }
}
