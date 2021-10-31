using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate
{
    [Command("controller", Order = 2, HelpText = "Gerar código de teste a partir de controlador.")]
    public class GenerateControllerCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Nome da classe controlador.")]
        public string Controller { get; set; }

        [Parameter(IsRequired = false, HelpText = "Nome da classe entidade.")]
        public string Entity { get; set; }

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        private readonly IControllerService _controllerService;
        private readonly IFileSystemInfraService _fileSystemInfraService;
        private readonly ICommandHandler _commandHandler;
        private readonly IFixtureService _fixtureService;
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly ITemplateCodeInfraService _templateCodeInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly ISettingInfraService _settingInfraService;

        public GenerateControllerCommand(
            IControllerService controllerService,
            ICommandHandler commandHandler,
            IFixtureService fixtureService,
            ISettingInfraService settingInfraService,
            ITestScanInfraService testScanInfraService,
            ITemplateCodeInfraService templateCodeInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            IFileSystemInfraService fileSystemInfraService
        ) : base() 
        {
            _commandHandler = commandHandler;
            _fixtureService = fixtureService;
            _controllerService = controllerService;
            _settingInfraService = settingInfraService;
            _testScanInfraService = testScanInfraService;
            _fileSystemInfraService = fileSystemInfraService;
            _templateCodeInfraService = templateCodeInfraService;
            _webApiScanInfraService = webApiScanInfraService;
        }

        public override async Task ExecuteAsync(ICommandContext context)
        {
            if (!context.ExecutionCascade)
            {
                if (!await _webApiScanInfraService.ProjectExistAsync())
                    throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);
                if (!await _testScanInfraService.ProjectExistAsync())
                    throw new ProjectNotFoundException(ProjectTypeEnumerator.INTEGRATION_TESTS);
                if (!await _testScanInfraService.ClassExistAsync(HelpClassEnumerator.TEST_BASE.Name))
                    throw new ClassNotFoundException(HelpClassEnumerator.TEST_BASE.Name);
                if (!await _testScanInfraService.ClassExistAsync(HelpClassEnumerator.ENTITY_FAKER_FACTORY.Name))
                    throw new ClassNotFoundException(HelpClassEnumerator.ENTITY_FAKER_FACTORY.Name);
            }
            var controllerName = _controllerService.GetControllerName(Controller);
            var controllerClass = await _webApiScanInfraService.GetControllerAsync(controllerName);
            if (controllerClass is null) throw new ClassNotFoundException(controllerName);
            var dbContextName = await _settingInfraService.GetStringAsync(SettingEnumerator.DB_CONTEXT_NAME);
            if (string.IsNullOrWhiteSpace(dbContextName))
                throw new ValidationException("Nenhuma classe de banco de dados configurada.");
            if (!await _webApiScanInfraService.ModelExistAsync(dbContextName)) 
                throw new ClassNotFoundException(dbContextName);
            var fixtureClassName = _fixtureService.GetFixtureName();
            if (!context.ExecutionCascade && !await _testScanInfraService.ClassExistAsync(fixtureClassName))
                throw new ClassNotFoundException(fixtureClassName);

            var controllerTestName = _controllerService.GetControllerTestName(controllerName);
            var filePath = GetControllerTestFilePath(controllerTestName);
            if (!context.ExecutionCascade && await _fileSystemInfraService.FileExistAsync(filePath))
                throw new DuplicatedSourceFileException(controllerTestName);

            var entityName = _controllerService.GetEntityName(controllerName, Entity);
            var dbSet = await _controllerService.GetDbSetClassAsync(dbContextName, entityName);
            var templateModel = await _controllerService.GetControllerTestAsync(controllerClass, dbSet);
            var commands = await _controllerService.GetRequiredCommandsAsync(templateModel, Static);
            await _commandHandler.HandleManyAsync(commands, true);

            var sourceCode = _templateCodeInfraService.BuildControllerTest(templateModel);
            await _fileSystemInfraService.SaveFileAsync(filePath, sourceCode);
        }

        protected string GetControllerTestFilePath(string controllerTestName)
        {
            var fileName = $"{controllerTestName}.cs";
            return Path.Combine(GetOutputDirectory(), fileName);
        }

        private string GetOutputDirectory() => string.IsNullOrWhiteSpace(Output) 
            ? _controllerService.GetDirectoryBase() : Output;
    }
}
