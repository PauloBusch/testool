using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Commands.Configure;
using TesTool.Core.Commands.Generate.Factory;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Common;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Commands.Generate
{
    [Command("project", Order = 1, HelpText = "Gerar código de teste a partir de projeto.")]
    public class GenerateProjectCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Diretório do projeto.")]
        public string ProjectPath { get; set; }

        [Parameter(IsRequired = false, HelpText = "Nome do contexto de banco de dados.")]
        public string DbContext { get; set; }

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        private readonly ICommandHandler _commandHandler;
        private readonly IFixtureService _fixtureService;
        private readonly IServiceResolver _serviceResolver;
        private readonly IFileSystemInfraService _fileSystemInfraService;
        private readonly ICommonXUnitRunnerService _commonXUnitRunnerService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ICommonProjectExplorerService _commonProjectExplorerService;
        private readonly ICommonConfigurationLoaderService _commonConfigurationLoaderService;
        private readonly ICommonAssertExtensionsService _commonAssertExtensionsService;
        private readonly ICommonEntityFakerBaseService _commonEntityFakerBaseService;
        private readonly ICommonTestBaseService _commonTestBaseService;
        private readonly IProjectInfraManager _projectInfraManager;
        private readonly ISolutionInfraService _solutionInfraService;
        private readonly ITestCodeInfraService _testCodeInfraService;
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly IWebApiDbContextInfraService _webApiDbContextInfraService;
        private readonly IEnvironmentInfraService _environmentInfraService;
        private readonly ITemplateCodeInfraService _templateCodeInfraService;
        private readonly ISettingInfraService _settingInfraService;

        public GenerateProjectCommand(
            ICommandHandler commandHandler,
            IFixtureService fixtureService,
            IServiceResolver serviceResolver,
            ILoggerInfraService loggerInfraService,
            ISettingInfraService settingInfraService,
            ICommonRequestService commonRequestService,
            ITestScanInfraService testScanInfraService,
            ICommonXUnitRunnerService commonXUnitRunnerService,
            ICommonProjectExplorerService commonProjectExplorerService,
            ICommonConfigurationLoaderService commonConfigurationLoaderService,
            ICommonAssertExtensionsService commonAssertExtensionsService,
            ICommonEntityFakerBaseService commonEntityFakerBaseService,
            ICommonTestBaseService commonTestBaseService,
            IProjectInfraManager projectInfraManager,
            ISolutionInfraService solutionInfraService,
            ITestCodeInfraService testCodeInfraService,
            IFileSystemInfraService fileSystemInfraService,
            IWebApiDbContextInfraService webApiDbContextInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            IEnvironmentInfraService environmentInfraService,
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(loggerInfraService)
        {
            _commandHandler = commandHandler;
            _fixtureService = fixtureService;
            _serviceResolver = serviceResolver;
            _fileSystemInfraService = fileSystemInfraService;
            _settingInfraService = settingInfraService;
            _testScanInfraService = testScanInfraService;
            _solutionInfraService = solutionInfraService;
            _testCodeInfraService = testCodeInfraService;
            _commonXUnitRunnerService = commonXUnitRunnerService;
            _commonRequestService = commonRequestService;
            _commonProjectExplorerService = commonProjectExplorerService;
            _commonConfigurationLoaderService = commonConfigurationLoaderService;
            _commonAssertExtensionsService = commonAssertExtensionsService;
            _commonEntityFakerBaseService = commonEntityFakerBaseService;
            _commonTestBaseService = commonTestBaseService;
            _projectInfraManager = projectInfraManager;
            _webApiDbContextInfraService = webApiDbContextInfraService;
            _webApiScanInfraService = webApiScanInfraService;
            _environmentInfraService = environmentInfraService;
            _templateCodeInfraService = templateCodeInfraService;
        }

        public override async Task GenerateAsync(ICommandContext context)
        {
            _settingInfraService.CreateTemporarySetting();
            await _commandHandler.HandleManyAsync(GetBeforeCommands(), true);

            if (!await _webApiScanInfraService.ProjectExistAsync())
                throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);

            var dbContextClass = await GetDbContextAsync();
            await _testCodeInfraService.CreateTestProjectAsync(_solutionInfraService.GetTestProjectName(), GetOutputDirectory());

            _loggerInfraService.LogInformation($"Gerando arquivos.");
            await SaveXUnitRunnerFileAsync();
            await SaveRequestClassAsync();
            await SaveProjectExplorerClassAsync();
            await SaveConfigurationLoaderClassAsync();
            await SaveAssertExtensionsClassAsync();
            
            var controllers = await _webApiScanInfraService.GetControllersAsync();
            await SaveTestBaseClassAsync(dbContextClass, controllers);
            await SaveFixtureClassAsync(dbContextClass);
            await SaveEntityFakerBaseClassAsync(dbContextClass);

            _testScanInfraService.ClearCache();
            await _commandHandler.HandleManyAsync(GetAfterCommands(dbContextClass), true);

            _testScanInfraService.ClearCache();
            await _commandHandler.HandleManyAsync(GetTestCommands(controllers), true);
        }

        private IEnumerable<ICommand> GetBeforeCommands()
        {
            var configureCommand = _serviceResolver.ResolveService<ConfigureWebApiProjectCommand>();
            configureCommand.ProjectPath = ProjectPath;

            return new ICommand[] { configureCommand };
        }

        private IEnumerable<ICommand> GetAfterCommands(Class dbContextClass)
        {
            var generateFactoryEntityCommand = _serviceResolver.ResolveService<GenerateFactoryEntityCommand>();
            generateFactoryEntityCommand.DbContext = dbContextClass.Name;

            return new ICommand[] {
                _serviceResolver.ResolveService<GenerateFactoryCompareCommand>(),
                _serviceResolver.ResolveService<GenerateFactoryModelCommand>(),
                generateFactoryEntityCommand
            };
        }

        private IEnumerable<ICommand> GetTestCommands(IEnumerable<Controller> controllers)
        {
            return controllers.Select(controller => {
                var generateControllerCommand = _serviceResolver.ResolveService<GenerateControllerCommand>();
                generateControllerCommand.Controller = controller.Name;
                generateControllerCommand.Static = Static;
                return generateControllerCommand;
            });
        }

        private async Task SaveXUnitRunnerFileAsync()
        {
            var xunitRunnerPathFile = _commonXUnitRunnerService.GetPathFile();
            var xunitRunnerSourceCode = _templateCodeInfraService.BuildXUnitRunner();
            if (await _fileSystemInfraService.FileExistAsync(xunitRunnerPathFile))
                throw new DuplicatedSourceFileException(xunitRunnerSourceCode);

            await _fileSystemInfraService.SaveFileAsync(xunitRunnerPathFile, xunitRunnerSourceCode);
            _projectInfraManager.AddFileCopyToOutput(_settingInfraService.ProjectIntegrationTestDirectory, Path.GetFileName(xunitRunnerPathFile));
        }

        private async Task SaveRequestClassAsync()
        {
            var requestPathFile = _commonRequestService.GetPathFile();
            var requestNamespace = _commonRequestService.GetNamespace();
            var requestSourceCode = _templateCodeInfraService.BuildHttpRequest(requestNamespace);
            if (await _fileSystemInfraService.FileExistAsync(requestPathFile))
                throw new DuplicatedSourceFileException(requestSourceCode);

            await _fileSystemInfraService.SaveFileAsync(requestPathFile, requestSourceCode);
        }

        private async Task SaveProjectExplorerClassAsync()
        {
            var projectExplorerPathFile = _commonProjectExplorerService.GetPathFile();
            var projectExplorerNamespace = _commonProjectExplorerService.GetNamespace();
            var projectExplorerSourceCode = _templateCodeInfraService.BuildProjectExplorer(projectExplorerNamespace);
            if (await _fileSystemInfraService.FileExistAsync(projectExplorerPathFile))
                throw new DuplicatedSourceFileException(projectExplorerSourceCode);

            await _fileSystemInfraService.SaveFileAsync(projectExplorerPathFile, projectExplorerSourceCode);
        }

        private async Task SaveConfigurationLoaderClassAsync()
        {
            var configurationLoaderPathFile = _commonConfigurationLoaderService.GetPathFile();
            var configurationLoaderNamespace = _commonConfigurationLoaderService.GetNamespace();
            var configurationLoaderSourceCode = _templateCodeInfraService.BuildConfigurationLoader(configurationLoaderNamespace);
            if (await _fileSystemInfraService.FileExistAsync(configurationLoaderPathFile))
                throw new DuplicatedSourceFileException(configurationLoaderSourceCode);

            await _fileSystemInfraService.SaveFileAsync(configurationLoaderPathFile, configurationLoaderSourceCode);
        }

        private async Task SaveAssertExtensionsClassAsync()
        {
            var assertExtensionsPathFile = _commonAssertExtensionsService.GetPathFile();
            var assertExtensionsNamespace = _commonAssertExtensionsService.GetNamespace();
            var assertExtensionsSourceCode = _templateCodeInfraService.BuildAssertExtensions(assertExtensionsNamespace);
            if (await _fileSystemInfraService.FileExistAsync(assertExtensionsPathFile))
                throw new DuplicatedSourceFileException(assertExtensionsSourceCode);

            await _fileSystemInfraService.SaveFileAsync(assertExtensionsPathFile, assertExtensionsSourceCode);
        }

        private async Task SaveFixtureClassAsync(Class dbContextClass)
        {
            var fixturePathFile = _fixtureService.GetPathFile();
            var fixtureModel = _fixtureService.GetModel(dbContextClass);
            var fixtureSourceCode = _templateCodeInfraService.BuildFixture(fixtureModel);
            if (await _fileSystemInfraService.FileExistAsync(fixturePathFile))
                throw new DuplicatedSourceFileException(fixtureSourceCode);

            await _fileSystemInfraService.SaveFileAsync(fixturePathFile, fixtureSourceCode);
        }

        private async Task SaveTestBaseClassAsync(Class dbContextClass, IEnumerable<Controller> controllers)
        {
            var testBasePathFile = _commonTestBaseService.GetPathFile();
            var testBaseModel = _commonTestBaseService.GetTestBaseModel(dbContextClass, controllers.Any(c => c.Authorize || c.Endpoints.Any(c => c.Authorize)));
            var testBaseSourceCode = _templateCodeInfraService.BuildTestBase(testBaseModel);
            if (await _fileSystemInfraService.FileExistAsync(testBasePathFile))
                throw new DuplicatedSourceFileException(testBaseSourceCode);

            await _fileSystemInfraService.SaveFileAsync(testBasePathFile, testBaseSourceCode);
        }

        private async Task SaveEntityFakerBaseClassAsync(Class dbContextClass)
        {
            var entityFakerBasePathFile = _commonEntityFakerBaseService.GetPathFile();
            var entityFakerBaseModel = _commonEntityFakerBaseService.GetEntityFakerBaseModel(dbContextClass);
            var entityFakerBaseSourceCode = _templateCodeInfraService.BuildEntityFakerBase(entityFakerBaseModel);
            if (await _fileSystemInfraService.FileExistAsync(entityFakerBasePathFile))
                throw new DuplicatedSourceFileException(entityFakerBaseSourceCode);

            await _fileSystemInfraService.SaveFileAsync(entityFakerBasePathFile, entityFakerBaseSourceCode);
        }

        private async Task<Class> GetDbContextAsync()
        {
            var dbContextName = string.IsNullOrWhiteSpace(DbContext) 
                ? _settingInfraService.DbContextName : DbContext;
            if (!string.IsNullOrWhiteSpace(dbContextName) && !await _webApiDbContextInfraService.IsDbContextClassAsync(dbContextName))
                throw new ValidationException("DbContext informado não é uma classe de contexto de banco de dados do Entity Framework.");
                
            var dbContextClasses = await _webApiDbContextInfraService.GetDbContextClassesAsync();
            if (!string.IsNullOrWhiteSpace(dbContextName))
                return dbContextClasses.Single(c => c.Name == dbContextName);
            if (dbContextClasses.Count() == 1) return dbContextClasses.Single();
            throw new ValidationException(
                "Várias classes de contexto de banco de dados foram encontradas. " +
                "Especifique qual classe deve ser utilizada."
            );
        }

        private string GetOutputDirectory() => string.IsNullOrWhiteSpace(Output)
            ? _environmentInfraService.GetWorkingDirectory() : Output;
    }
}
