using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Fakers;
using TesTool.Core.Models.Configuration;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Services.Fakers
{
    public class FakeEntityService : FakeServiceBase, IFakeEntityService
    {
        private readonly IConventionInfraService _conventionInfraService;

        public FakeEntityService(
            ISolutionService solutionService,
            ITestScanInfraService testScanInfraService,
            IExpressionInfraService expressionInfraService,
            IConventionInfraService conventionInfraService
        ) : base(
            solutionService, testScanInfraService,
            expressionInfraService
        )
        {
            _conventionInfraService = conventionInfraService;
        }

        public async Task<ModelFaker> GetFakerModelAsync(Class model, bool @static)
        {
            var conventions = await _conventionInfraService.GetConfiguredConventionsAsync();
            var templateModel = new ModelFaker(model.Name, GetNamespace());
            templateModel.AddNamespace(model.Namespace);

            foreach (var property in model.Properties)
            {
                if (property.Type is Field field)
                {
                    var bogusExpression = GetBogusExpression(property, field, conventions);
                    if (string.IsNullOrWhiteSpace(bogusExpression)) continue;
                    if (@static) bogusExpression = await _expressionInfraService.BuildBogusExpressionAsync(bogusExpression);

                    var bogusProperty = new ModelProperty(property.Name, bogusExpression, false);
                    templateModel.AddProperty(bogusProperty);
                }
                else if (property.Type is Enum enumType)
                {
                    templateModel.AddNamespace(enumType.Namespace);
                    if (@static)
                    {
                        var bogusProperty = new ModelProperty(property.Name, $"{enumType.Name}.{enumType.Values.Last().Key}", false);
                        templateModel.AddProperty(bogusProperty);
                    }
                    else
                    {
                        var expression = BogusMethodEnumerator.RANDOM_ENUM.Expression.Replace("{ENUM_NAME}", enumType.Name);
                        var bogusProperty = new ModelProperty(property.Name, expression, false);
                        templateModel.AddProperty(bogusProperty);
                    }
                }
                else if (property.Type is Class propertyType)
                {
                    var fakerName = $"{propertyType.Name}Faker";
                    var existingFaker = await _testScanInfraService.GetClassAsync(fakerName);
                    var expression = BogusMethodEnumerator.COMPLEX_OBJECT.Expression.Replace("{FAKER_NAME}", fakerName);
                    var bogusProperty = new ModelProperty(property.Name, expression, existingFaker is null);
                    if (existingFaker is not null) templateModel.AddNamespace(existingFaker.Namespace);
                    templateModel.AddProperty(bogusProperty);
                }
                else if (property.Type is Array array && array.Type is Class arrayType)
                {
                    var fakerName = $"{arrayType.Name}Faker";
                    var existingFaker = await _testScanInfraService.GetClassAsync(fakerName);
                    var expression = BogusMethodEnumerator.COLLECTION.Expression.Replace("{FAKER_NAME}", fakerName);
                    var bogusProperty = new ModelProperty(property.Name, expression, existingFaker is null);
                    if (existingFaker is not null) templateModel.AddNamespace(existingFaker.Namespace);
                    templateModel.AddProperty(bogusProperty);
                }
            }

            return templateModel;
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestNamespace("Fakers.Models");
        }

        private string GetBogusExpression(
            Property property, Field field, 
            IEnumerable<Convention> conventions
        ) {
            var convention = conventions.LastOrDefault(c =>
                (string.IsNullOrWhiteSpace(c.TypeMatch) || Regex.IsMatch(field.SystemType, c.TypeMatch, RegexOptions.IgnoreCase)) &&
                (string.IsNullOrWhiteSpace(c.PropertyMatch) || Regex.IsMatch(property.Name, c.PropertyMatch, RegexOptions.IgnoreCase))
            );
            return convention?.BogusExpression;
        }
    }
}
