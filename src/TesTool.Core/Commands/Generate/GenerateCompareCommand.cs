using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Comparator;
using TesTool.Core.Models.Templates.Factory;

namespace TesTool.Core.Commands.Generate
{
    // TODO: move to GenerateFactoryCompareCommand
    [Command("compare", Order = 4, HelpText = "Gerar código de comparação entre objetos.")]
    public class GenerateCompareCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Nome da classe de origem.")]
        public string SourceClassName { get; set; }

        [Parameter(HelpText = "Nome da classe de destino.")]
        public string TargetClassName { get; set; }

        // TODO: remove and get default
        [Parameter(IsRequired = false, HelpText = "Nome da classe que terá o método de comparação.")]
        public string ComparatorName { get; set; }

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        private readonly ITestScanInfraService _testScanInfraService;
        private readonly ITestCodeInfraService _testCodeInfraService;
        private readonly ITemplateCodeInfraService _templateCodeInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly IFileSystemInfraService _fileSystemInfraService;

        public GenerateCompareCommand(
            ITestScanInfraService testScanInfraService,
            ITestCodeInfraService testCodeInfraService,
            ITemplateCodeInfraService templateCodeInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            IEnvironmentInfraService environmentInfraService,
            IFileSystemInfraService fileSystemInfraService
        ) : base(environmentInfraService)
        {
            _testScanInfraService = testScanInfraService;
            _testCodeInfraService = testCodeInfraService;
            _templateCodeInfraService = templateCodeInfraService;
            _webApiScanInfraService = webApiScanInfraService;
            _fileSystemInfraService = fileSystemInfraService;
        }

        public override async Task ExecuteAsync()
        {
            if (!string.IsNullOrWhiteSpace(Output) && !Directory.Exists(Output))
                throw new DirectoryNotFoundException("Diretório de saída inválido.");

            if (!await _testScanInfraService.ProjectExistAsync())
                throw new ProjectNotFoundException(ProjectTypeEnumerator.INTEGRATION_TESTS);

            var comparatorClass = await TryGetComparatorClassAsync(SourceClassName, TargetClassName);
            if (comparatorClass is not null) throw new DuplicatedClassException(comparatorClass.Name);

            var sourceModel = await _webApiScanInfraService.GetModelAsync(SourceClassName) as Class;
            var targetModel = await _webApiScanInfraService.GetModelAsync(TargetClassName) as Class;
            if (targetModel is null) throw new ClassNotFoundException(SourceClassName);
            if (sourceModel is null) throw new ClassNotFoundException(TargetClassName);

            var fileName = $"{GetComparatorName()}.cs";
            var filePath = Path.Combine(GetOutputDirectory(), fileName);
            if (await _fileSystemInfraService.FileExistAsync(filePath))
                throw new DuplicatedSourceFileException(fileName);

            string sourceCode;
            if (Static)
            {
                var templateModel = await MapModelCompareStaticAsync(sourceModel, targetModel);
                sourceCode = _templateCodeInfraService.ProcessComparerStatic(templateModel);
            } else
            {
                var templateModel = await MapModelCompareDynamicAsync(sourceModel, targetModel);
                await CreateAssertExtensionAsync(templateModel);
                sourceCode = _templateCodeInfraService.ProcessComparerDynamic(templateModel);
            }
            await _fileSystemInfraService.SaveFileAsync(filePath, sourceCode);

            var exist = await _testScanInfraService.ClassExistAsync(ComparatorName);
            if (exist) await AppendComparatorFactoryMethodAsync();
            else await CreateComparatorFactoryClassAsync();
        }

        private async Task<CompareStatic> MapModelCompareStaticAsync(Class source, Class target)
        {
            var templateModel = new CompareStatic(GetComparatorNamespace(), GetComparatorName(), SourceClassName, TargetClassName);
            templateModel.AddNamespace(source.Namespace);
            templateModel.AddNamespace(target.Namespace);

            var propertiesToAssert = source.Properties.Where(p => target.Properties.Any(p1 => p1.Name == p.Name && p1.Type.Wrapper == p.Type.Wrapper));
            if (!propertiesToAssert.Any()) throw new ValidationException("None properties to assert Equals.");

            foreach (var sourceProperty in propertiesToAssert)
            {
                var targetProperty = target.Properties.Single(p => p.Name == sourceProperty.Name && p.Type.Wrapper == sourceProperty.Type.Wrapper);
                if (sourceProperty.Type is Field || sourceProperty.Type is Enum)
                {
                    var propertyCompare = new CompareProperty(sourceProperty.Name);
                    templateModel.AddProperty(propertyCompare);
                }
                else if (sourceProperty.Type is Class sourceClass)
                {
                    var targetClass = targetProperty.Type as Class;
                    var comparatorClass = await TryGetComparatorClassAsync(sourceClass.Name, targetClass.Name);
                    if (comparatorClass is not null) templateModel.AddNamespace(comparatorClass.Namespace);
                    var comparatorClassName = comparatorClass?.Name ?? $"{sourceClass.Name}Equals{targetClass.Name}";

                    var comparerObject = new CompareObject(sourceProperty.Name, comparatorClassName, comparatorClass is null);
                    templateModel.AddComparer(comparerObject);
                }
            }

            return templateModel;
        }

        private async Task<CompareDynamic> MapModelCompareDynamicAsync(Class source, Class target)
        {
            var templateModel = new CompareDynamic(GetComparatorNamespace(), GetComparatorName(), SourceClassName, TargetClassName);
            templateModel.AddNamespace(source.Namespace);
            templateModel.AddNamespace(target.Namespace);
            return await Task.FromResult(templateModel);
        }

        private string GetComparatorName()
        {
            return $"{SourceClassName}Equals{TargetClassName}";
        }

        public async Task CreateComparatorFactoryClassAsync()
        {
            var templateModel = GetModelFactory();
            var fileName = $"{ComparatorName}.cs";
            var filePath = Path.Combine(GetOutputDirectory(), fileName);
            var factorySourceCode = _templateCodeInfraService.ProcessComparerFactory(templateModel);
            if (await _fileSystemInfraService.FileExistAsync(filePath))
                throw new DuplicatedSourceFileException(fileName);

            await _fileSystemInfraService.SaveFileAsync(filePath, factorySourceCode);
        }

        public async Task AppendComparatorFactoryMethodAsync() {
            var templateModel = GetModelFactory();
            var factoryPathFile = await _testScanInfraService.GetPathClassAsync(ComparatorName);
            var factorySourceCode = _templateCodeInfraService.ProcessComparerFactory(templateModel);
            var mergedSourceCode = await _testCodeInfraService.MergeClassCodeAsync(ComparatorName, factorySourceCode);
            await _fileSystemInfraService.SaveFileAsync(factoryPathFile, mergedSourceCode);
        }

        private async Task CreateAssertExtensionAsync(CompareDynamic templateModel)
        {
            var extensionClassName = "AssertExtensions";
            var extensionClass = await _testScanInfraService.GetClassAsync(extensionClassName);
            if (extensionClass is null)
            {
                var extensionFileName = $"{extensionClassName}.cs";
                var extensionNamespace = GetExtensionNamespace();
                var extensionFilePath = Path.Combine(GetOutputDirectory(), extensionFileName);
                var extensionSourceCode = _templateCodeInfraService.ProcessAssertExtensions(extensionNamespace);
                if (await _fileSystemInfraService.FileExistAsync(extensionFilePath))
                    throw new DuplicatedSourceFileException(extensionFileName);

                await _fileSystemInfraService.SaveFileAsync(extensionFilePath, extensionSourceCode);
                templateModel.AddNamespace(extensionNamespace);
                return;
            }

            templateModel.AddNamespace(extensionClass.Namespace);
        }

        private ComparatorFactory GetModelFactory()
        {
            var templateModel = new ComparatorFactory(ComparatorName, GetComparatorFactoryNamespace(), Static);
            templateModel.AddNamespace(GetComparatorNamespace());
            templateModel.AddMethod(new ComparatorFactoryMethod(GetComparatorName()));
            return templateModel;
        }

        private async Task<Class> TryGetComparatorClassAsync(string sourceClassName, string targetClassName)
        {
            var comparatorClassName1 = $"{sourceClassName}Equals{targetClassName}";
            var comparatorClass1 = await _testScanInfraService.GetClassAsync(comparatorClassName1);
            if (comparatorClass1 is not null) return comparatorClass1; 

            var comparatorClassName2 = $"{targetClassName}Equals{sourceClassName}";
            var comparatorClass2 = await _testScanInfraService.GetClassAsync(comparatorClassName2);
            if (comparatorClass2 is not null) return comparatorClass2;

            return default;
        }

        private string GetExtensionNamespace() => GetNamespace("Extensions");
        private string GetComparatorNamespace() => GetNamespace("Assertions.Comparators");
        private string GetComparatorFactoryNamespace() => GetNamespace("Assertions");

        private string GetNamespace(string sufix)
        {
            var integrationTestNamespace = _testScanInfraService.GetNamespace();
            if (!string.IsNullOrWhiteSpace(integrationTestNamespace)) return $"{integrationTestNamespace}.{sufix}";

            var webApiNamespace = _webApiScanInfraService.GetNamespace();
            return $"{webApiNamespace}.IntegrationTests.{sufix}";
        }
    }
}
