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

        private readonly ISettingInfraService _settingInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly IWebApiDbContextInfraService _webApiDbContextInfraService;
        private readonly IFactoryEntityService _factoryEntityService;

        public GenerateFactoryEntityCommand(
            IFactoryEntityService factoryEntityService,
            ITestScanInfraService testScanInfraService,
            ISettingInfraService settingInfraService,
            IWebApiDbContextInfraService webApiDbContextInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            IFileSystemInfraService fileSystemInfraService, 
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            HelpClassEnumerator.ENTITY_FAKER_FACTORY,
            testScanInfraService, fileSystemInfraService, templateCodeInfraService
        ) 
        {
            _settingInfraService = settingInfraService;
            _webApiDbContextInfraService = webApiDbContextInfraService;
            _factoryEntityService = factoryEntityService;
            _webApiScanInfraService = webApiScanInfraService;
        }

        protected override async Task GenerateAsync()
        {
            if (!await _testScanInfraService.ClassExistAsync(HelpClassEnumerator.TEST_BASE.Name))
                throw new ClassNotFoundException(HelpClassEnumerator.TEST_BASE.Name);
            if (!await _webApiScanInfraService.ProjectExistAsync())
                throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);
            if (!await _webApiScanInfraService.ModelExistAsync(DbContext))
                throw new ClassNotFoundException(DbContext);
            if (!await _webApiDbContextInfraService.IsDbContextClassAsync(DbContext))
                throw new ValidationException("DbContext informado não é uma classe de contexto de banco de dados do Entity Framework.");

            await base.GenerateAsync();
            await _settingInfraService.SetStringAsync(SettingEnumerator.DB_CONTEXT_NAME, DbContext);
        }

        protected override string BuildSourceCode(string factoryName)
        {
            var templateModel = _factoryEntityService.GetEntityFactoryAsync(factoryName, DbContext).Result;
            return _templateCodeInfraService.BuildEntityFakerFactory(templateModel);
        }

        protected override string GetOutputDirectory() => string.IsNullOrWhiteSpace(Output)
            ? _factoryEntityService.GetDirectoryBase() : Output;
    }
}
