using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Commands.Configure;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
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
        private readonly ICommonRequestService _commonRequestService;
        private readonly ICommonProjectExplorerService _commonProjectExplorerService;
        private readonly ICommonConfigurationLoaderService _commonConfigurationLoaderService;
        private readonly ISolutionInfraService _solutionInfraService;
        private readonly ITestCodeInfraService _testCodeInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly IWebApiDbContextInfraService _webApiDbContextInfraService;
        private readonly IEnvironmentInfraService _environmentInfraService;
        private readonly ITemplateCodeInfraService _templateCodeInfraService;
        private readonly ISettingInfraService _settingInfraService;

        public GenerateProjectCommand(
            ICommandHandler commandHandler,
            IFixtureService fixtureService,
            IServiceResolver serviceResolver,
            ISettingInfraService settingInfraService,
            ICommonRequestService commonRequestService,
            ICommonProjectExplorerService commonProjectExplorerService,
            ICommonConfigurationLoaderService commonConfigurationLoaderService,
            ISolutionInfraService solutionInfraService,
            ITestCodeInfraService testCodeInfraService,
            IFileSystemInfraService fileSystemInfraService,
            IWebApiDbContextInfraService webApiDbContextInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            IEnvironmentInfraService environmentInfraService,
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(fileSystemInfraService)
        {
            _commandHandler = commandHandler;
            _fixtureService = fixtureService;
            _serviceResolver = serviceResolver;
            _settingInfraService = settingInfraService;
            _solutionInfraService = solutionInfraService;
            _testCodeInfraService = testCodeInfraService;
            _commonRequestService = commonRequestService;
            _commonProjectExplorerService = commonProjectExplorerService;
            _commonConfigurationLoaderService = commonConfigurationLoaderService;
            _webApiDbContextInfraService = webApiDbContextInfraService;
            _webApiScanInfraService = webApiScanInfraService;
            _environmentInfraService = environmentInfraService;
            _templateCodeInfraService = templateCodeInfraService;
        }

        protected override async Task GenerateAsync()
        {
            var configureCommand = _serviceResolver.ResolveService<ConfigureWebApiProjectCommand>();
            configureCommand.ProjectPath = ProjectPath;
            await _commandHandler.HandleAsync(configureCommand, true);

            if (!await _webApiScanInfraService.ProjectExistAsync())
                throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);

            await _testCodeInfraService.CreateTestProjectAsync(_solutionInfraService.GetTestName(), GetOutputDirectory());

            await SaveRequestClassAsync();
            await SaveProjectExplorerClassAsync();
            await SaveConfigurationLoaderClassAsync();
            await SaveFixtureClassAsync(await GetDbContextAsync());
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

        private async Task SaveFixtureClassAsync(Class dbContextClass)
        {
            var fullPath = _fixtureService.GetFixturePathFile();
            var fixtureModel = _fixtureService.GetFixtureModel(dbContextClass);
            var fixtureSourceCode = _templateCodeInfraService.BuildFixture(fixtureModel);
            if (await _fileSystemInfraService.FileExistAsync(fullPath))
                throw new DuplicatedSourceFileException(fixtureSourceCode);

            await _fileSystemInfraService.SaveFileAsync(fullPath, fixtureSourceCode);
        }

        private async Task<Class> GetDbContextAsync()
        {
            var dbContextName = string.IsNullOrWhiteSpace(DbContext) 
                ? await _settingInfraService.GetStringAsync(SettingEnumerator.DB_CONTEXT_NAME)
                : DbContext;
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
