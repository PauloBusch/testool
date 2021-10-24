using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("model", HelpText = "Gerar fábrica de modelo de transporte de dados (DTO).")]
    public class GenerateFactoryModelCommand : GenerateFactoryBase
    {
        private readonly IFactoryModelService _factoryModelService;

        public GenerateFactoryModelCommand(
            IFactoryModelService factoryModelService,
            ITestScanInfraService testScanInfraService,
            IFileSystemInfraService fileSystemInfraService,
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            TestClassEnumerator.MODEL_FAKER_FACTORY,
            testScanInfraService, fileSystemInfraService, 
            templateCodeInfraService
        ) 
        {
            _factoryModelService = factoryModelService;
        }

        protected override string BuildSourceCode(string factoryName)
        {
            var templateModel = _factoryModelService.GetModelFactory(factoryName);
            return _templateCodeInfraService.BuildModelFakerFactory(templateModel);
        }

        protected override string GetOutputDirectory() => string.IsNullOrWhiteSpace(Output)
            ? _factoryModelService.GetDirectoryBase() : Output;
    }
}
