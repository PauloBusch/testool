using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("entity", HelpText = "Gerar fábrica de entidades de banco de dados.")]
    public class GenerateFactoryEntityCommand : GenerateFactoryBase
    {
        [Parameter(HelpText = "Nome do contexto de banco de dados.")]
        public string DbContext { get; set; }

        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly IFactoryEntityService _factoryEntityService;

        public GenerateFactoryEntityCommand(
            IFactoryEntityService factoryEntityService,
            ISettingInfraService settingInfraService, 
            IEnvironmentInfraService environmentInfraService, 
            ITestScanInfraService testScanInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            IFileSystemInfraService fileSystemInfraService, 
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            SettingEnumerator.ENTITY_FAKER_FACTORY_NAME,
            TestClassEnumerator.ENTITY_FAKER_FACTORY,
            settingInfraService, environmentInfraService, 
            testScanInfraService, fileSystemInfraService, 
            templateCodeInfraService
        ) 
        {
            _factoryEntityService = factoryEntityService;
            _webApiScanInfraService = webApiScanInfraService;
        }

        protected override async Task GenerateAsync()
        {
            if (!await _testScanInfraService.ClassExistAsync(TestClassEnumerator.TEST_BASE.Name))
                throw new ClassNotFoundException(TestClassEnumerator.TEST_BASE.Name);
            if (!await _webApiScanInfraService.ProjectExistAsync())
                throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);
            if (!await _webApiScanInfraService.ModelExistAsync(DbContext))
                throw new ClassNotFoundException(DbContext);
            if (!await _webApiScanInfraService.IsContextEntityFramework(DbContext))
                throw new ValidationException("DbContext informado não é uma classe de contexto de banco de dados do Entity Framework.");

            await base.GenerateAsync();
            await _settingInfraService.SetStringAsync(SettingEnumerator.DB_CONTEXT_NAME, DbContext);
        }

        protected override string BuildSourceCode(string factoryName)
        {
            var templateModel = _factoryEntityService.GetEntityFactoryAsync(factoryName, DbContext).Result;
            return _templateCodeInfraService.BuildEntityFakerFactory(templateModel);
        }
    }
}
