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
            ITestScanInfraService testScanInfraService,
            IFileSystemInfraService fileSystemInfraService,
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            HelpClassEnumerator.COMPARE_FACTORY,
            testScanInfraService, fileSystemInfraService, templateCodeInfraService
        ) 
        { 
            _factoryCompareService = factoryCompareService;    
        }

        protected override string BuildSourceCode(string factoryName)
        {
            var templateModel = _factoryCompareService.GetModelFactory(factoryName);
            return _templateCodeInfraService.BuildComparatorFactory(templateModel);
        }

        protected override string GetOutputDirectory() => string.IsNullOrWhiteSpace(Output)
            ? _factoryCompareService.GetDirectoryBase() : Output;
    }
}
