using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Configuration;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Factory;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Commands.Generate
{
    [Command("fake", "f", HelpText = "Gerar código de fabricação de objeto.")]
    public class GenerateFakeCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Nome da classe a ser fábricada.")]
        public string ClassName { get; set; }

        [Parameter(HelpText = "Nome da classe que terá o método de fabricação.")]
        public string FactoryName { get; set; }

        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly IIntegrationTestScanInfraService _integrationTestScanInfraService;
        private readonly ITemplateCodeInfraService _templateCodeInfraService;
        private readonly IFileSystemInfraService _fileSystemInfraService;
        private readonly IEnvironmentInfraService _environmentInfraService;
        private readonly IConventionInfraService _conventionInfraService;
        private readonly IExpressionInfraService _expressionInfraService;

        public GenerateFakeCommand(
            IWebApiScanInfraService webApiScanInfraService,
            IIntegrationTestScanInfraService integrationTestScanInfraService,
            IEnvironmentInfraService environmentInfraService,
            ITemplateCodeInfraService templateCodeInfraService,
            IFileSystemInfraService fileSystemInfraService,
            IConventionInfraService conventionInfraService,
            IExpressionInfraService expressionInfraService
        )
        {
            _webApiScanInfraService = webApiScanInfraService;
            _fileSystemInfraService = fileSystemInfraService;
            _environmentInfraService = environmentInfraService;
            _integrationTestScanInfraService = integrationTestScanInfraService;
            _templateCodeInfraService = templateCodeInfraService;
            _conventionInfraService = conventionInfraService;
            _expressionInfraService = expressionInfraService;
        }

        public async override Task ExecuteAsync()
        {
            if (!string.IsNullOrWhiteSpace(Output) && !Directory.Exists(Output)) 
                throw new DirectoryNotFoundException("Diretório de saída inválido.");

            if (!await _integrationTestScanInfraService.ProjectExistAsync())
                throw new ProjectNotFoundException(ProjectTypeEnumerator.INTEGRATION_TESTS);

            var model = await _webApiScanInfraService.GetModelAsync(ClassName);
            if (model is null) throw new ModelNotFoundException(ClassName);
            if (model is Class dto)
            {
                var fakerName = $"{ClassName}Faker";
                //if (await _integrationTestScanInfraService.ClassExistAsync(fakerName))
                //    throw new DuplicatedClassException(fakerName);

                var fileName = $"{dto.Name}Faker.cs";
                var filePath = Path.Combine(GetOutputDirectory(), fileName);
                var sourceCode = _templateCodeInfraService.ProcessFaker(await MapTemplateModelAsync(dto));
                //if (await _fileSystemInfraService.FileExistAsync(filePath))
                //    throw new DuplicatedSourceFileException(fileName);

                await _fileSystemInfraService.SaveFileAsync(filePath, sourceCode);
                
                var exist = await _integrationTestScanInfraService.ClassExistAsync(FactoryName);
                if (exist) await AppendFactoryMethodAsync();
                else await CreateFactoryClassAsync();
            } else throw new ValidationException("This class is not a model.");
        }

        private async Task<Bogus> MapTemplateModelAsync(Class model)
        {
            var conventions = await _conventionInfraService.GetConfiguredConventionsAsync();
            var templateModel = new Bogus(model.Name, GetFakerNamespace());
            templateModel.AddNamespace(model.Namespace);

            foreach (var property in model.Properties)
            {
                if (property.Type is Field field)
                {
                    var bogusExpression = await GetBogusExpressionAsync(property, field, conventions);
                    if (string.IsNullOrWhiteSpace(bogusExpression)) continue;

                    var bogusProperty = new BogusProperty(property.Name, bogusExpression, false);
                    templateModel.AddProperty(bogusProperty);
                }
                else if (property.Type is Enum enumType)
                {
                    templateModel.AddNamespace(enumType.Namespace);
                    if (Static)
                    {
                        var bogusProperty = new BogusProperty(property.Name, $"{enumType.Name}.{enumType.Values.Last().Key}", false);
                        templateModel.AddProperty(bogusProperty);
                    } else
                    {
                        var expression = BogusMethodEnumerator.RANDOM_ENUM.Expression.Replace("{ENUM_NAME}", enumType.Name);
                        var bogusProperty = new BogusProperty(property.Name, expression, false);
                        templateModel.AddProperty(bogusProperty);
                    }
                }
                else if (property.Type is Class propertyType)
                {
                    var fakerName = $"{propertyType.Name}Faker";
                    var existingFaker = await _integrationTestScanInfraService.GetClassAsync(fakerName);
                    var expression = BogusMethodEnumerator.COMPLEX_OBJECT.Expression.Replace("{FAKER_NAME}", fakerName);
                    var bogusProperty = new BogusProperty(property.Name, expression, existingFaker is null);
                    if (existingFaker is not null) templateModel.AddNamespace(existingFaker.Namespace);
                    templateModel.AddProperty(bogusProperty);
                } else if (property.Type is Array array && array.Type is Class arrayType)
                {
                    var fakerName = $"{arrayType.Name}Faker";
                    var existingFaker = await _integrationTestScanInfraService.GetClassAsync(fakerName);
                    var expression = BogusMethodEnumerator.COLLECTION.Expression.Replace("{FAKER_NAME}", fakerName);
                    var bogusProperty = new BogusProperty(property.Name, expression, existingFaker is null);
                    if (existingFaker is not null) templateModel.AddNamespace(existingFaker.Namespace);
                    templateModel.AddProperty(bogusProperty);
                }
            }

            return templateModel;
        }

        private async Task<string> GetBogusExpressionAsync(Property property, Field field, IEnumerable<Convention> conventions)
        {
            var convention = conventions.LastOrDefault(c => 
                (string.IsNullOrWhiteSpace(c.TypeMatch) || Regex.IsMatch(field.SystemType, c.TypeMatch, RegexOptions.IgnoreCase)) &&
                (string.IsNullOrWhiteSpace(c.PropertyMatch) || Regex.IsMatch(property.Name, c.PropertyMatch, RegexOptions.IgnoreCase))
            );
            if (convention is null) return default; 
            if (Static) return await _expressionInfraService.BuildBogusExpressionAsync(convention.BogusExpression);
            return convention.BogusExpression;
        }

        private async Task CreateFactoryClassAsync()
        {
            var templateModel = GetModelFactory();
            var fileName = $"{FactoryName}.cs";
            var filePath = Path.Combine(GetOutputDirectory(), fileName);
            var factorySourceCode = _templateCodeInfraService.ProcessFakerFactory(templateModel);
            //if (await _fileSystemInfraService.FileExistAsync(filePath))
            //    throw new DuplicatedSourceFileException(fileName);

            await _fileSystemInfraService.SaveFileAsync(filePath, factorySourceCode);
        }

        private async Task AppendFactoryMethodAsync()
        {
            var templateModel = GetModelFactory();
            var factoryPathFile = await _integrationTestScanInfraService.GetPathClassAsync(FactoryName);
            var factorySourceCode = _templateCodeInfraService.ProcessFakerFactory(templateModel);
            var mergedSourceCode = await _integrationTestScanInfraService.MergeClassCodeAsync(FactoryName, factorySourceCode);
            await _fileSystemInfraService.SaveFileAsync(factoryPathFile, mergedSourceCode);
        }

        private ModelFactory GetModelFactory()
        {
            var templateModel = new ModelFactory(FactoryName, GetFakerFactoryNamespace());
            templateModel.AddNamespace(GetFakerNamespace());
            templateModel.AddMethod(new ModelFactoryMethod(ClassName, $"{ClassName}Faker"));
            return templateModel;
        }

        private string GetFakerNamespace()
        {
            var integrationTestNamespace = _integrationTestScanInfraService.GetNamespace();
            if (!string.IsNullOrWhiteSpace(integrationTestNamespace)) return $"{integrationTestNamespace}.Fakers.Models";

            var webApiNamespace = _webApiScanInfraService.GetNamespace();
            return $"{webApiNamespace}.IntegrationTests.Fakers.Models";
        }

        private string GetFakerFactoryNamespace()
        {
            var integrationTestNamespace = _integrationTestScanInfraService.GetNamespace();
            if (!string.IsNullOrWhiteSpace(integrationTestNamespace)) return $"{integrationTestNamespace}.Fakers";

            var webApiNamespace = _webApiScanInfraService.GetNamespace();
            return $"{webApiNamespace}.IntegrationTests.Fakers";
        }

        private string GetOutputDirectory() => string.IsNullOrWhiteSpace(Output) 
            ? _environmentInfraService.GetWorkingDirectory() 
            : Output;
    }
}
