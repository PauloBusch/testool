using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("entity", HelpText = "Gerar fábrica de entidades de banco de dados.")]
    public class GenerateFactoryEntityCommand : GenerateFactoryBase
    {
        [Parameter(HelpText = "Nome do contexto de banco de dados.")]
        public string DbContext { get; set; }

        private readonly IWebApiScanInfraService _webApiScanInfraService;

        public GenerateFactoryEntityCommand(
            ISettingInfraService settingInfraService, 
            IEnvironmentInfraService environmentInfraService, 
            ITestScanInfraService testScanInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            IFileSystemInfraService fileSystemInfraService, 
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            SettingEnumerator.ENTITY_FACTORY_NAME,
            TestClassEnumerator.ENTITY_FACTORY,
            settingInfraService, environmentInfraService, 
            testScanInfraService, fileSystemInfraService, 
            templateCodeInfraService
        ) 
        {
            _webApiScanInfraService = webApiScanInfraService;
        }

        protected override async Task GenerateAsync()
        {
            if (!await _webApiScanInfraService.ProjectExistAsync())
                throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);
            if (!await _webApiScanInfraService.ExistModelAsync(DbContext))
                throw new ClassNotFoundException(DbContext);
            if (!await _webApiScanInfraService.IsContextEntityFramework(DbContext))
                throw new ValidationException("DbContext informado não é uma classe de contexto de banco de dados do Entity Framework.");

            await base.GenerateAsync();
        }

        protected override string BuildSourceCode(string factoryName)
        {
            throw new System.NotImplementedException();
        }
    }
}
