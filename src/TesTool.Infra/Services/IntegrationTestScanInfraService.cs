using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;

namespace TesTool.Infra.Services
{
    public class IntegrationTestScanInfraService : ProjectScanInfraServiceBase, IIntegrationTestScanInfraService
    {
        private readonly IEnvironmentInfraService _environmentInfraService;

        public IntegrationTestScanInfraService(
            ILoggerInfraService loggerInfraService,
            IEnvironmentInfraService environmentInfraService
        ) : base(ProjectTypeEnumerator.INTEGRATION_TESTS, loggerInfraService) 
        { 
            _environmentInfraService = environmentInfraService;
        }

        public async Task<Dto> GetClassAsync(string name)
        {
            var project = GetProject();
            if (project is null) return default;

            var dtoClass = null as Dto;
            await ForEachClassesAsync((@class, root, model) => {
                if (dtoClass is not null) return; 

                var type = GetModelType(model.GetDeclaredSymbol(@class) as ITypeSymbol);
                if (type is Dto dto && dto.Name == name) dtoClass = dto;
            });

            return dtoClass;
        }

        private string _cacheProjectPath;
        protected override string GetProjectPath()
        {
            if (!string.IsNullOrWhiteSpace(_cacheProjectPath)) return _cacheProjectPath;
            
            var applicationBasePath = _environmentInfraService.GetWorkingDirectory();
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var projectDirectoryInfo = new DirectoryInfo(directoryInfo.FullName);
                if (projectDirectoryInfo.Exists)
                {
                    var projectFiles = Directory.GetFiles(projectDirectoryInfo.FullName, "*.csproj");
                    if (projectFiles.Any()) {
                        _cacheProjectPath = projectFiles.First();
                        return _cacheProjectPath;
                    }
                }
                
                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            return default;
        }
    }
}
