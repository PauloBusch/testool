using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Configuration;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("factory", Order = 3, HelpText = "Gerar código de chamadas de fabricação.")]
    public abstract class GenerateFactoryBase : GenerateCommandBase
    {
        [Parameter(IsRequired = false, HelpText = "Nome da classe fábrica.")]
        public string FactoryName { get; set; }
        
        private readonly Setting _setting;
        private readonly TestClass _testClass;
        private readonly ISettingInfraService _settingInfraService;

        protected readonly IFileSystemInfraService _fileSystemInfraService;
        protected readonly ITemplateCodeInfraService _templateCodeInfraService;
        protected readonly ITestScanInfraService _testScanInfraService;

        protected GenerateFactoryBase(
            Setting setting,
            TestClass testClass,
            ISettingInfraService settingInfraService,
            IEnvironmentInfraService environmentInfraService,
            ITestScanInfraService testScanInfraService,
            IFileSystemInfraService fileSystemInfraService,
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(environmentInfraService) 
        {
            _setting = setting;
            _testClass = testClass;
            _settingInfraService = settingInfraService;
            _fileSystemInfraService = fileSystemInfraService;
            _templateCodeInfraService = templateCodeInfraService;
            _testScanInfraService = testScanInfraService;
        }

        protected abstract string BuildSourceCode(string factoryName);

        public override async Task ExecuteAsync()
        {
            var factoryName = await GetFactoryNameAsync();
            if (await _testScanInfraService.ClassExistAsync(factoryName))
                throw new DuplicatedClassException(factoryName);

            var filePath = await GetFactoryFilePathAsync();
            var factorySourceCode = BuildSourceCode(factoryName);
            if (await _fileSystemInfraService.FileExistAsync(filePath))
                throw new DuplicatedSourceFileException(factoryName);

            await _fileSystemInfraService.SaveFileAsync(filePath, factorySourceCode);
            await _settingInfraService.SetStringAsync(_setting.Key, factoryName);
        }

        private async Task<string> GetFactoryNameAsync()
        {
            if (!string.IsNullOrWhiteSpace(FactoryName)) return FactoryName;

            return await _settingInfraService.GetStringAsync(_setting.Key) ?? _testClass.Name;
        }

        private async Task<string> GetFactoryFilePathAsync()
        {
            var fileName = $"{await GetFactoryNameAsync()}.cs";
            return Path.Combine(GetOutputDirectory(), fileName);
        }
    }
}
