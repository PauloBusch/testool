﻿using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("model", HelpText = "Gerar fábrica de modelo de transporte de dados (DTO).")]
    public class GenerateFactoryModelCommand : GenerateFactoryBase
    {
        protected readonly IFactoryModelService _factoryModelService;

        public GenerateFactoryModelCommand(
            ISettingInfraService settingInfraService,
            IFactoryModelService factoryModelService,
            ITestScanInfraService testScanInfraService,
            IFileSystemInfraService fileSystemInfraService,
            ITemplateCodeInfraService templateCodeInfraService,
            IEnvironmentInfraService environmentInfraService
        ) : base(
            SettingEnumerator.MODEL_FACTORY_NAME, 
            TestClassEnumerator.MODEL_FACTORY,
            settingInfraService, environmentInfraService, 
            testScanInfraService, fileSystemInfraService, 
            templateCodeInfraService
        ) 
        {
            _factoryModelService = factoryModelService;
        }

        protected override string BuildSourceCode(string factoryName)
        {
            var templateModel = _factoryModelService.GetModelFactory(factoryName);
            return _templateCodeInfraService.BuildModelFactory(templateModel);
        }
    }
}