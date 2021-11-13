using System.Text.RegularExpressions;
using TesTool.Core.Enumerations;
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

        public string GetPathFile()
        {
            var projectName = _webApiScanInfraService.GetName();
            var fixtureName = $"{Regex.Replace(projectName, @"\W", string.Empty)}Fixture";
            return @$"{_testScanInfraService.GetDirectoryBase()}\{fixtureName}.cs";
        }

        public Fixture GetModel(Class dbContextClass)
        {
            var fixtureModel = new Fixture(
                dbContextClass,
                GetName(),
                _webApiScanInfraService.GetName(),
                GetNamespace()
            );
            fixtureModel.AddNamespace(_webApiScanInfraService.GetNamespace());
            fixtureModel.AddNamespace(_commonRequestService.GetNamespace());
            fixtureModel.AddNamespace(_commonProjectExplorerService.GetNamespace());
            fixtureModel.AddNamespace(_commonConfigurationLoaderService.GetNamespace());
            return fixtureModel;
        }

        public string GetName()
        {
            var projectName = _webApiScanInfraService.GetName();
            var normalizedProjectName = Regex.Replace(projectName, @"\W", string.Empty);
            return HelpClassEnumerator.FIXTURE.Name.Replace("{PROJECT_NAME}", normalizedProjectName);
        }

        public string GetNamespace()
        {
            return $"{_webApiScanInfraService.GetNamespace()}.IntegrationTests";
        }
    }
}
