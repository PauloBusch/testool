using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Commands.Generate.Fakers
{
    [Command("faker", Order = 5, HelpText = "Gerar código de fabricação de objeto.")]
    public abstract class GenerateFakeBase : GenerateCommandBase
    {
        [Parameter(HelpText = "Nome da classe a ser fábricada.")]
        public string ClassName { get; set; }

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        protected readonly IWebApiScanInfraService _webApiScanInfraService;
        protected readonly IFileSystemInfraService _fileSystemInfraService;
        protected readonly ITestScanInfraService _testScanInfraService;
        protected readonly ITemplateCodeInfraService _templateCodeInfraService;

        public GenerateFakeBase(
            ILoggerInfraService loggerInfraService,
            IFileSystemInfraService fileSystemInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            ITestScanInfraService testScanInfraService,
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(loggerInfraService)
        {
            _webApiScanInfraService = webApiScanInfraService;
            _fileSystemInfraService = fileSystemInfraService;
            _testScanInfraService = testScanInfraService;
            _templateCodeInfraService = templateCodeInfraService;
        }

        protected abstract string GetFactoryName();
        protected abstract string GetFakerName(string className);
        protected abstract string GetOutputDirectory();
        protected abstract Task<string> BuildSourceCodeAsync(Class @class, string fakerName);
        protected abstract Task AppendFactoryMethodAsync(Class @class, string fakerName, string factoryName);

        public async override Task GenerateAsync(ICommandContext context)
        {
            if (!context.ExecutionCascade)
            {
                if (!await _webApiScanInfraService.ProjectExistAsync())
                    throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);
                if (!await _testScanInfraService.ProjectExistAsync())
                    throw new ProjectNotFoundException(ProjectTypeEnumerator.INTEGRATION_TESTS);

                var factoryNameValidate = GetFactoryName();
                if (!await _testScanInfraService.ClassExistAsync(factoryNameValidate))
                    throw new ClassNotFoundException(factoryNameValidate);
            }

            Class @class = await _webApiScanInfraService.GetModelAsync(ClassName) as Class;
            if (@class is null) throw new ModelNotFoundException(ClassName);

            var fakerName = GetFakerName(ClassName);
            var filePath = GetFakerFilePath();
            if (!context.ExecutionCascade)
            {
                if (await _testScanInfraService.ClassExistAsync(fakerName))
                    throw new DuplicatedClassException(fakerName);

                if (await _fileSystemInfraService.FileExistAsync(filePath))
                    throw new DuplicatedSourceFileException(fakerName);
            }

            var fakerSourceCode = await BuildSourceCodeAsync(@class, fakerName);
            await _fileSystemInfraService.SaveFileAsync(filePath, fakerSourceCode);
            await AppendFactoryMethodAsync(@class, fakerName, GetFactoryName());
        }

        protected string GetFakerFilePath()
        {
            var fileName = $"{GetFakerName(ClassName)}.cs";
            return Path.Combine(GetOutputDirectory(), fileName);
        }
    }
}
