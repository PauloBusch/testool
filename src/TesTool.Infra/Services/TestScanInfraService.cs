using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;

namespace TesTool.Infra.Services
{
    public class TestScanInfraService : ProjectScanInfraServiceBase, ITestScanInfraService
    {
        private readonly IProjectInfraManager _projectExplorer;

        protected readonly ISettingInfraService _settingInfraService;
        protected readonly IEnvironmentInfraService _environmentInfraService;

        public TestScanInfraService(
            IProjectInfraManager projectExplorer,
            ISettingInfraService settingInfraService,
            IEnvironmentInfraService environmentInfraService
        ) : base(ProjectTypeEnumerator.INTEGRATION_TESTS) 
        { 
            _projectExplorer = projectExplorer;
            _settingInfraService = settingInfraService;
            _environmentInfraService = environmentInfraService;
        }

        public async Task<bool> ClassExistAsync(string className)
        {
            var classes = await GetClassesAsync();
            return classes.Any(c => c.Declaration.Identifier.Text == className);
        }

        public async Task<Class> GetClassAsync(string className)
        {
            var classes = await GetClassesAsync();
            var @class = classes.FirstOrDefault(c => c.Declaration.Identifier.Text == className);
            if (@class is null) return default;
            
            return GetModelType(@class.TypeSymbol) as Class;
        }

        public async Task<string> GetPathClassAsync(string className)
        {
            var classes = await GetClassesAsync();
            return classes.FirstOrDefault(c => c.Declaration.Identifier.Text == className)?.FilePath;
        }

        public string GetDirectoryBase()
        {
            var projectPathFile = GetProjectPathFile();
            return Path.GetDirectoryName(projectPathFile);
        }

        private static string _cacheProjectPath;
        public override string GetProjectPathFile()
        {
            if (!string.IsNullOrWhiteSpace(_cacheProjectPath)) return _cacheProjectPath;

            var settingProjectPathFile = _settingInfraService.ProjectIntegrationTestDirectory;
            if (!string.IsNullOrWhiteSpace(settingProjectPathFile) && _projectExplorer.IsTestProjectFile(settingProjectPathFile))
            {
                _cacheProjectPath = settingProjectPathFile;
                return settingProjectPathFile;
            }

            _cacheProjectPath = _projectExplorer.GetCurrentProject(_projectExplorer.IsTestProjectFile);
            return _cacheProjectPath;
        }
    }
}
