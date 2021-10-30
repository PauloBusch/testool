using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Configuration;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Services.Fakers
{
    public abstract class FakeServiceBase
    {
        protected readonly ISolutionInfraService _solutionService;
        protected readonly ITestScanInfraService _testScanInfraService;
        protected readonly IExpressionInfraService _expressionInfraService;
        protected readonly IConventionInfraService _conventionInfraService;

        public FakeServiceBase(
            ISolutionInfraService solutionService,
            ITestScanInfraService testScanInfraService,
            IExpressionInfraService expressionInfraService,
            IConventionInfraService conventionInfraService
        )
        {
            _solutionService = solutionService;
            _testScanInfraService = testScanInfraService;
            _expressionInfraService = expressionInfraService;
            _conventionInfraService = conventionInfraService;
        }

        public string GetFakerName(string className)
        {
            return $"{className}Faker";
        }

        protected abstract T MapProperty<T>(string name, string expression, bool @unsafe) where T : PropertyBase;

        protected async Task<T> FillTemplateAsync<T, TProperty>(T templateModel, Class model, bool @static) 
            where T : FakerBase<TProperty>
            where TProperty : PropertyBase
        {
            templateModel.AddNamespace(model.Namespace);
            var conventions = await _conventionInfraService.GetConfiguredConventionsAsync();
            foreach (var property in model.Properties)
            {
                if (property.Type is Field field)
                {
                    var bogusExpression = GetBogusExpression(property, field, conventions);
                    if (string.IsNullOrWhiteSpace(bogusExpression)) continue;
                    if (@static) bogusExpression = await _expressionInfraService.BuildBogusExpressionAsync(bogusExpression);

                    templateModel.AddProperty(MapProperty<TProperty>(property.Name, bogusExpression, false));
                }
                else if (property.Type is Enum enumType)
                {
                    templateModel.AddNamespace(enumType.Namespace);
                    if (@static)
                    {
                        var bogusProperty = MapProperty<TProperty>(property.Name, $"{enumType.Name}.{enumType.Values.Last().Key}", false);
                        templateModel.AddProperty(bogusProperty);
                    }
                    else
                    {
                        var expression = BogusMethodEnumerator.RANDOM_ENUM.Expression.Replace("{ENUM_NAME}", enumType.Name);
                        var bogusProperty = MapProperty<TProperty>(property.Name, expression, false);
                        templateModel.AddProperty(bogusProperty);
                    }
                }
                else if (property.Type is Class propertyType)
                {
                    var fakerName = $"{propertyType.Name}Faker";
                    var existingFaker = await _testScanInfraService.GetClassAsync(fakerName);
                    var expression = BogusMethodEnumerator.COMPLEX_OBJECT.Expression.Replace("{FAKER_NAME}", fakerName);
                    var bogusProperty = MapProperty<TProperty>(property.Name, expression, existingFaker is null);
                    if (existingFaker is not null) templateModel.AddNamespace(existingFaker.Namespace);
                    templateModel.AddProperty(bogusProperty);
                }
                else if (property.Type is Array array && array.Type is Class arrayType)
                {
                    var fakerName = $"{arrayType.Name}Faker";
                    var existingFaker = await _testScanInfraService.GetClassAsync(fakerName);
                    var expression = BogusMethodEnumerator.COLLECTION.Expression.Replace("{FAKER_NAME}", fakerName);
                    var bogusProperty = MapProperty<TProperty>(property.Name, expression, existingFaker is null);
                    if (existingFaker is not null) templateModel.AddNamespace(existingFaker.Namespace);
                    templateModel.AddProperty(bogusProperty);
                }
            }

            return templateModel;
        }

        private string GetBogusExpression(
            Property property, Field field,
            IEnumerable<Convention> conventions
        )
        {
            var convention = conventions.LastOrDefault(c =>
                (string.IsNullOrWhiteSpace(c.TypeMatch) || Regex.IsMatch(field.SystemType, c.TypeMatch, RegexOptions.IgnoreCase)) &&
                (string.IsNullOrWhiteSpace(c.PropertyMatch) || Regex.IsMatch(property.Name, c.PropertyMatch, RegexOptions.IgnoreCase))
            );
            return convention?.BogusExpression;
        }
    }
}
