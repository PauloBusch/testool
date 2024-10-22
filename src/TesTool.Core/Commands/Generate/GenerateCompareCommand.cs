﻿using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Common;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Commands.Generate
{
    [Command("compare", Order = 4, HelpText = "Gerar código de comparação entre objetos.")]
    public class GenerateCompareCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Nome da classe de origem.")]
        public string SourceClassName { get; set; }

        [Parameter(HelpText = "Nome da classe de destino.")]
        public string TargetClassName { get; set; }

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        private readonly ICompareService _compareService;
        private readonly IFileSystemInfraService _fileSystemInfraService;
        private readonly IFactoryCompareService _factoryCompareService;
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly ITestCodeInfraService _testCodeInfraService;
        private readonly ITemplateCodeInfraService _templateCodeInfraService;
        private readonly ICommonAssertExtensionsService _commonAssertExtensionsService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;

        public GenerateCompareCommand(
            ICompareService compareService,
            ILoggerInfraService loggerInfraService,
            IFactoryCompareService factoryCompareService,
            ITestScanInfraService testScanInfraService,
            ITestCodeInfraService testCodeInfraService,
            ITemplateCodeInfraService templateCodeInfraService,
            ICommonAssertExtensionsService commonAssertExtensionsService,
            IWebApiScanInfraService webApiScanInfraService,
            IFileSystemInfraService fileSystemInfraService
        ) : base(loggerInfraService)
        {
            _compareService = compareService;
            _factoryCompareService = factoryCompareService;
            _testScanInfraService = testScanInfraService;
            _testCodeInfraService = testCodeInfraService;
            _templateCodeInfraService = templateCodeInfraService;
            _fileSystemInfraService = fileSystemInfraService;
            _commonAssertExtensionsService = commonAssertExtensionsService;
            _webApiScanInfraService = webApiScanInfraService;
        }

        public override async Task GenerateAsync(ICommandContext context)
        {
            if (!context.ExecutionCascade)
            {
                if (!await _webApiScanInfraService.ProjectExistAsync())
                    throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);
                if (!await _testScanInfraService.ProjectExistAsync())
                    throw new ProjectNotFoundException(ProjectTypeEnumerator.INTEGRATION_TESTS);

                var factoryName = _factoryCompareService.GetFactoryName();
                if (!await _testScanInfraService.ClassExistAsync(factoryName))
                    throw new ClassNotFoundException(factoryName);

                var comparatorClass = await _compareService.GetComparatorClassAsync(SourceClassName, TargetClassName);
                if (comparatorClass is not null) throw new DuplicatedClassException(comparatorClass.Name);
            }

            Class sourceModel = await _webApiScanInfraService.GetModelAsync(SourceClassName) as Class;
            Class targetModel = await _webApiScanInfraService.GetModelAsync(TargetClassName) as Class;
            if (targetModel is null) throw new ClassNotFoundException(SourceClassName);
            if (sourceModel is null) throw new ClassNotFoundException(TargetClassName);

            var comparatorName = GetComparatorName();
            var filePath = GetComparatorFilePath();
            if (!context.ExecutionCascade && await _fileSystemInfraService.FileExistAsync(filePath))
                throw new DuplicatedSourceFileException(comparatorName);

            string sourceCode;
            if (Static)
            {
                var templateModel = await _compareService.GetCompareStaticAsync(sourceModel, targetModel);
                sourceCode = _templateCodeInfraService.BuildCompareStatic(templateModel);
            } else
            {
                var templateModel = await _compareService.GetCompareDynamicAsync(sourceModel, targetModel);
                var extensionClassName = HelpClassEnumerator.ASSERT_EXTENSIONS.Name;
                if (!await _testScanInfraService.ClassExistAsync(extensionClassName)) 
                    throw new ClassNotFoundException(extensionClassName);

                templateModel.AddNamespace(_commonAssertExtensionsService.GetNamespace());
                sourceCode = _templateCodeInfraService.BuildCompareDynamic(templateModel);
            }
            
            await _fileSystemInfraService.SaveFileAsync(filePath, sourceCode);
            await AppendComparatorFactoryMethodAsync();
        }

        public async Task AppendComparatorFactoryMethodAsync() {
            var templateModel = _factoryCompareService.GetModelFactory(_factoryCompareService.GetFactoryName());
            templateModel.AddNamespace(_compareService.GetNamespace());
            templateModel.AddMethod(new ComparatorFactoryMethod(GetComparatorName()));

            var factoryPathFile = await _testScanInfraService.GetPathClassAsync(templateModel.Name);
            var factorySourceCode = _templateCodeInfraService.BuildComparatorFactory(templateModel);
            var mergedSourceCode = await _testCodeInfraService.MergeClassCodeAsync(templateModel.Name, factorySourceCode);
            await _fileSystemInfraService.SaveFileAsync(factoryPathFile, mergedSourceCode);
        }

        private string GetComparatorName()
        {
            return _compareService.GetComparatorName(SourceClassName, TargetClassName);
        }

        protected string GetComparatorFilePath()
        {
            var fileName = $"{GetComparatorName()}.cs";
            return Path.Combine(GetOutputDirectory(), fileName);
        }

        private string GetOutputDirectory() => string.IsNullOrWhiteSpace(Output)
            ? _compareService.GetDirectoryBase() : Output;
    }
}
