using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("factory", Order = 3, HelpText = "Gerar código de chamadas de fabricação.")]
    public abstract class GenerateFactoryBase : GenerateCommandBase
    {        
        private readonly HelpClass _testClass;

        protected readonly IFileSystemInfraService _fileSystemInfraService;
        protected readonly ITemplateCodeInfraService _templateCodeInfraService;
        protected readonly ITestScanInfraService _testScanInfraService;

        protected GenerateFactoryBase(
            HelpClass testClass,
            ITestScanInfraService testScanInfraService,
            IFileSystemInfraService fileSystemInfraService,
            ITemplateCodeInfraService templateCodeInfraService
        ) : base() 
        {
            _testClass = testClass;
            _fileSystemInfraService = fileSystemInfraService;
            _templateCodeInfraService = templateCodeInfraService;
            _testScanInfraService = testScanInfraService;
        }

        protected abstract string BuildSourceCode(string factoryName);
        protected abstract string GetOutputDirectory();

        public override async Task ExecuteAsync(ICommandContext context)
        {
            if (!context.ExecutionCascade)
            {
                if (!await _testScanInfraService.ProjectExistAsync())
                    throw new ProjectNotFoundException(ProjectTypeEnumerator.INTEGRATION_TESTS);

                var factoryNameValidate = GetFactoryName();
                if (await _testScanInfraService.ClassExistAsync(factoryNameValidate))
                    throw new DuplicatedClassException(factoryNameValidate);
            }

            var filePath = GetFactoryFilePath();
            var factoryName = GetFactoryName();
            var factorySourceCode = BuildSourceCode(factoryName);
            if (!context.ExecutionCascade && await _fileSystemInfraService.FileExistAsync(filePath))
                throw new DuplicatedSourceFileException(factoryName);

            await _fileSystemInfraService.SaveFileAsync(filePath, factorySourceCode);
        }

        private string GetFactoryName()
        {
            return  _testClass.Name;
        }

        private string GetFactoryFilePath()
        {
            var fileName = $"{GetFactoryName()}.cs";
            return Path.Combine(GetOutputDirectory(), fileName);
        }
    }
}
