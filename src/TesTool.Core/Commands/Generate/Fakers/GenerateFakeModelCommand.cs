using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Interfaces.Services.Fakers;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Commands.Generate.Fakers
{
    [Command("model", HelpText = "Gerar código de fabricação de modelo de transporte de dados (DTO).")]
    public class GenerateFakeModelCommand : GenerateFakeBase
    {
        private readonly IFakeModelService _fakeModelService;
        private readonly IFactoryModelService _factoryModelService;
        private readonly ITestCodeInfraService _testCodeInfraService;

        public GenerateFakeModelCommand(
            IFakeModelService fakeEntityService,
            IFactoryModelService factoryModelService,
            ITestCodeInfraService testCodeInfraService,
            IFileSystemInfraService fileSystemInfraService, 
            IWebApiScanInfraService webApiScanInfraService, 
            ITestScanInfraService testScanInfraService, 
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            fileSystemInfraService, webApiScanInfraService, 
            testScanInfraService, templateCodeInfraService
        )
        {
            _fakeModelService = fakeEntityService;
            _factoryModelService = factoryModelService;
            _testCodeInfraService = testCodeInfraService;
        }

        protected override string GetFactoryName()
        {
            return _factoryModelService.GetFactoryName();
        }

        protected override async Task<string> BuildSourceCodeAsync(Class @class, string fakerName)
        {
            var templateModel = await _fakeModelService.GetFakerModelAsync(@class, Static);
            return _templateCodeInfraService.BuildModelFaker(templateModel);
        }

        protected override async Task AppendFactoryMethodAsync(Class @class, string fakerName, string factoryName)
        {
            var factoryTemplateModel = _factoryModelService.GetModelFactory(factoryName);
            factoryTemplateModel.AddNamespace(_fakeModelService.GetNamespace());
            factoryTemplateModel.AddMethod(new ModelFakerFactoryMethod(ClassName, fakerName));

            var factoryPathFile = await _testScanInfraService.GetPathClassAsync(factoryName);
            var factorySourceCode = _templateCodeInfraService.BuildModelFakerFactory(factoryTemplateModel);
            var mergedSourceCode = await _testCodeInfraService.MergeClassCodeAsync(factoryName, factorySourceCode);
            await _fileSystemInfraService.SaveFileAsync(factoryPathFile, mergedSourceCode);
        }

        protected override string GetFakerName(string className)
        {
            return _fakeModelService.GetFakerName(className);
        }

        protected override string GetOutputDirectory() => string.IsNullOrWhiteSpace(Output)
            ? _fakeModelService.GetDirectoryBase() : Output;
    }
}
