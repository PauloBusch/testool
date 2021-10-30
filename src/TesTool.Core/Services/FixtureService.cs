using System.Text.RegularExpressions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Common;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Common;

namespace TesTool.Core.Services
{
    public class FixtureService : IFixtureService
    {
        private readonly ICommonRequestService _commonRequestService;
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly ICommonProjectExplorerService _commonProjectExplorerService;
        private readonly ICommonConfigurationLoaderService _commonConfigurationLoaderService;

        public FixtureService(
            ICommonRequestService commonRequestService,
            IWebApiScanInfraService webApiScanInfraService,
            ICommonProjectExplorerService commonProjectExplorerService,
            ICommonConfigurationLoaderService commonConfigurationLoaderService,
            ITestScanInfraService testScanInfraService
        )
        {
            _commonRequestService = commonRequestService;
            _webApiScanInfraService = webApiScanInfraService;
            _commonProjectExplorerService = commonProjectExplorerService;
            _commonConfigurationLoaderService = commonConfigurationLoaderService;
            _testScanInfraService = testScanInfraService;
        }

        public string GetFixturePathFile()
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
            fixtureModel.AddNamespace(_commonRequestService.GetNamespace());
            fixtureModel.AddNamespace(_commonProjectExplorerService.GetNamespace());
            fixtureModel.AddNamespace(_commonConfigurationLoaderService.GetNamespace());
            return fixtureModel;
        }
    }
}
