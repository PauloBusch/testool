using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("compare", HelpText = "Gerar fábrica de objetos de comparação.")]
    public class GenerateFactoryCompareCommand : GenerateFactoryBase
    {
        private readonly IFactoryCompareService _factoryCompareService;

        public GenerateFactoryCompareCommand(
            IFactoryCompareService factoryCompareService,
            ISettingInfraService settingInfraService,
            IEnvironmentInfraService environmentInfraService,
            ITestScanInfraService testScanInfraService,
            IFileSystemInfraService fileSystemInfraService,
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            SettingEnumerator.COMPARATOR_FACTORY_NAME,
            TestClassEnumerator.COMPARE_FACTORY,
            settingInfraService, environmentInfraService,
            testScanInfraService, fileSystemInfraService,
            templateCodeInfraService
        ) 
        { 
            _factoryCompareService = factoryCompareService;    
        }

        protected override string BuildSourceCode(string factoryName)
        {
            var templateModel = _factoryCompareService.GetModelFactory(factoryName);
            return _templateCodeInfraService.BuildComparatorFactory(templateModel);
        }
    }
}
