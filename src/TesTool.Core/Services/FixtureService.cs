using System.Text.RegularExpressions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Common;

namespace TesTool.Core.Services
{
    public class FixtureService : IFixtureService
    {
        private readonly ISolutionInfraService _solutionService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly IWebApiDbContextInfraService _webApiDbContextInfraService;
        private readonly ITestScanInfraService _testScanInfraService;

        public FixtureService(
            ISolutionInfraService solutionService,
            IWebApiScanInfraService webApiScanInfraService, 
            IWebApiDbContextInfraService webApiDbContextInfraService, 
            ITestScanInfraService testScanInfraService
        )
        {
            _solutionService = solutionService;
            _webApiScanInfraService = webApiScanInfraService;
            _webApiDbContextInfraService = webApiDbContextInfraService;
            _testScanInfraService = testScanInfraService;
        }

        public string GetFixtureFullPath()
        {
            var projectName = _webApiScanInfraService.GetName();
            var fixtureName = $"{Regex.Replace(projectName, @"\W", string.Empty)}Fixture";
            return @$"{_testScanInfraService.GetDirectoryBase()}\{fixtureName}.cs";
        }

        public Fixture GetFixtureModel(Class dbContextClass)
        {
            var fixtureModel = new Fixture(
                dbContextClass,
                _webApiScanInfraService.GetName(),
                $"{_webApiScanInfraService.GetNamespace()}.IntegrationTests"
            );

            return fixtureModel;
        }
    }
}
