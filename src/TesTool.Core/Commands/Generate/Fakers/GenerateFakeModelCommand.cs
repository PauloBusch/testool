using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Interfaces.Services.Fakers;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Factory;

namespace TesTool.Core.Commands.Generate.Fakers
{
    [Command("model", HelpText = "Gerar código de fabricação de modelo de transporte de dados (DTO).")]
    public class GenerateFakeModelCommand : GenerateFakeBase
    {
        private readonly IFakeEntityService _fakeEntityService;
        private readonly IFactoryModelService _factoryModelService;
        private readonly ITestCodeInfraService _testCodeInfraService;

        public GenerateFakeModelCommand(
            IFakeEntityService fakeEntityService,
            IFactoryModelService factoryModelService,
            ITestCodeInfraService testCodeInfraService,
            IEnvironmentInfraService environmentInfraService, 
            IFileSystemInfraService fileSystemInfraService, 
            IWebApiScanInfraService webApiScanInfraService, 
            ITestScanInfraService testScanInfraService, 
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            environmentInfraService, 
            fileSystemInfraService, webApiScanInfraService, 
            testScanInfraService, templateCodeInfraService
        )
        {
            _fakeEntityService = fakeEntityService;
            _factoryModelService = factoryModelService;
            _testCodeInfraService = testCodeInfraService;
        }

        protected override async Task<string> GetFactoryNameAsync()
        {
            return await _factoryModelService.GetFactoryNameAsync();
        }

        protected override async Task<string> BuildSourceCodeAsync(Class @class, string fakerName)
        {
            var templateModel = await _fakeEntityService.GetFakerModelAsync(@class, Static);
            return _templateCodeInfraService.BuildModel(templateModel);
        }

        protected override async Task AppendFactoryMethodAsync(Class @class, string fakerName, string factoryName)
        {
            var factoryTemplateModel = _factoryModelService.GetModelFactory(factoryName);
            factoryTemplateModel.AddNamespace(_fakeEntityService.GetNamespace());
            factoryTemplateModel.AddMethod(new ModelFactoryMethod(ClassName, fakerName));

            var factoryPathFile = await _testScanInfraService.GetPathClassAsync(factoryName);
            var factorySourceCode = _templateCodeInfraService.BuildModelFactory(factoryTemplateModel);
            var mergedSourceCode = await _testCodeInfraService.MergeClassCodeAsync(factoryName, factorySourceCode);
            await _fileSystemInfraService.SaveFileAsync(factoryPathFile, mergedSourceCode);
        }
    }
}
