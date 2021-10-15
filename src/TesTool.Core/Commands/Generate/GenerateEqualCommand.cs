using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Comparator;

namespace TesTool.Core.Commands.Generate
{
    [Command("Equals", "e", HelpText = "Gerar código de comparação entre objetos.")]
    public class GenerateEqualsCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Nome da classe de origem.")]
        public string SourceClassName { get; set; }

        [Parameter(HelpText = "Nome da classe de destino.")]
        public string TargetClassName { get; set; }

        [Parameter(HelpText = "Nome da classe que terá o método de comparação.")]
        public string ComparatorName { get; set; }

        private readonly IIntegrationTestScanInfraService _integrationTestScanInfraService;
        private readonly ITemplateCodeInfraService _templateCodeInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly IFileSystemInfraService _fileSystemInfraService;

        public GenerateEqualsCommand(
            IIntegrationTestScanInfraService integrationTestScanInfraService,
            ITemplateCodeInfraService templateCodeInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            IEnvironmentInfraService environmentInfraService,
            IFileSystemInfraService fileSystemInfraService
        ) : base(environmentInfraService)
        {
            _integrationTestScanInfraService = integrationTestScanInfraService;
            _templateCodeInfraService = templateCodeInfraService;
            _webApiScanInfraService = webApiScanInfraService;
            _fileSystemInfraService = fileSystemInfraService;
        }

        public override async Task ExecuteAsync()
        {
            var comparatorClass = await TryGetComparatorClassAsync(SourceClassName, TargetClassName);
            if (comparatorClass is not null) throw new DuplicatedClassException(comparatorClass.Name);

            var sourceModel = await _webApiScanInfraService.GetModelAsync(SourceClassName) as Class;
            var targetModel = await _webApiScanInfraService.GetModelAsync(TargetClassName) as Class;
            if (targetModel is null) throw new ClassNotFoundException(SourceClassName);
            if (sourceModel is null) throw new ClassNotFoundException(TargetClassName);

            var fileName = $"{GetComparatorName()}.cs";
            var filePath = Path.Combine(GetOutputDirectory(), fileName);
            var sourceCode = _templateCodeInfraService.ProcessComparer(await MapModelCompareAsync(sourceModel, targetModel));
            //if (await _fileSystemInfraService.FileExistAsync(filePath))
            //    throw new DuplicatedSourceFileException(fileName);

            System.Console.WriteLine(sourceCode);
            await _fileSystemInfraService.SaveFileAsync(filePath, sourceCode);

            var exist = await _integrationTestScanInfraService.ClassExistAsync(ComparatorName);
            if (exist) await AppendComparatorMethodAsync();
            else await CreateComparatorClassAsync();
        }

        private async Task<ModelCompare> MapModelCompareAsync(Class source, Class target)
        {
            var templateModel = new ModelCompare(GetComparatorNamespace(), GetComparatorName(), SourceClassName, TargetClassName);
            templateModel.AddNamespace(source.Namespace);
            templateModel.AddNamespace(target.Namespace);

            var propertiesToAssert = source.Properties.Where(p => target.Properties.Any(p1 => p1.Name == p.Name && p1.Type.Wrapper == p.Type.Wrapper));
            if (!propertiesToAssert.Any()) throw new ValidationException("None properties to assert Equals.");

            foreach (var sourceProperty in propertiesToAssert)
            {
                var targetProperty = target.Properties.Single(p => p.Name == sourceProperty.Name && p.Type.Wrapper == sourceProperty.Type.Wrapper);
                // TODO: Review for array comparation
                if (sourceProperty.Type is Field || sourceProperty.Type is Enum || sourceProperty.Type is Array)
                {
                    var propertyCompare = new ModelCompareProperty(sourceProperty.Name);
                    templateModel.AddProperty(propertyCompare);
                }
                else if (sourceProperty.Type is Class sourceClass)
                {
                    var targetClass = targetProperty.Type as Class;
                    var comparatorClass = await TryGetComparatorClassAsync(sourceClass.Name, targetClass.Name);
                    if (comparatorClass is not null) templateModel.AddNamespace(comparatorClass.Namespace);
                    var comparatorClassName = comparatorClass?.Name ?? $"{sourceClass.Name}Equals{targetClass.Name}";

                    var comparerObject = new ModelCompareObject(sourceProperty.Name, comparatorClassName, comparatorClass is null);
                    templateModel.AddComparer(comparerObject);
                }
            }

            return templateModel;
        }

        private string GetComparatorName()
        {
            return $"{SourceClassName}Equals{TargetClassName}";
        }

        public async Task AppendComparatorMethodAsync() { }

        public async Task CreateComparatorClassAsync() { }

        private async Task<Class> TryGetComparatorClassAsync(string sourceClassName, string targetClassName)
        {
            var comparatorClassName1 = $"{sourceClassName}Equals{targetClassName}";
            var comparatorClass1 = await _integrationTestScanInfraService.GetClassAsync(comparatorClassName1);
            if (comparatorClass1 is not null) return comparatorClass1; 

            var comparatorClassName2 = $"{targetClassName}Equals{sourceClassName}";
            var comparatorClass2 = await _integrationTestScanInfraService.GetClassAsync(comparatorClassName2);
            if (comparatorClass2 is not null) return comparatorClass2;

            return default;
        }

        private string GetComparatorNamespace()
        {
            var integrationTestNamespace = _integrationTestScanInfraService.GetNamespace();
            if (!string.IsNullOrWhiteSpace(integrationTestNamespace)) return $"{integrationTestNamespace}.Comparators";

            var webApiNamespace = _webApiScanInfraService.GetNamespace();
            return $"{webApiNamespace}.IntegrationTests.Comparators";
        }
    }
}
