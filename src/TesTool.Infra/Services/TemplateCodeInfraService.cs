using System.Collections.Generic;
using System.Linq;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Templates.Comparator;
using TesTool.Core.Models.Templates.Factory;
using TesTool.Infra.Templates;

namespace TesTool.Infra.Services
{
    public class TemplateCodeInfraService : ITemplateCodeInfraService
    {
        public string ProcessFaker(Core.Models.Templates.Faker.Bogus model)
        {
            var template = new FakerTemplate
            {
                Name = model.Name,
                FakerNamespace = model.FakerNamespace,
                Namespaces = PrepareNamespaces(model.Namespaces, model.FakerNamespace),
                Properties = model.Properties.ToArray()
            };
            return template.TransformText();
        }

        public string ProcessFakerFactory(ModelFactory model)
        {
            var template = new ModelFakerFactoryTemplate 
            { 
                TemplataService = this,
                Name = model.Name,
                Methods = model.Methods.ToArray(),
                Namespaces = PrepareNamespaces(model.Namespaces, model.FactoryNamespace),
                FactoryNamespace = model.FactoryNamespace,
            };
            return template.TransformText();
        }

        public string ProcessFakerFactoryMethod(ModelFactoryMethod model)
        {
            var template = new ModelFakerFactoryMethodTemplate { Method = model };
            return template.TransformText();
        }

        public string ProcessComparer(ModelCompare model)
        {
            var template = new ModelComparerTemplate
            {
                Comparers = model.Comparers.ToArray(),
                Properties = model.Properties.ToArray(),
                ComparatorNamespace = model.ComparatorNamespace,
                ComparatorClassName = model.ComparatorClassName,
                SourceClassName = model.SourceClassName,
                TargetClassName = model.TargetClassName,
                Namespaces = PrepareNamespaces(model.Namespaces, model.ComparatorNamespace)
            };
            return template.TransformText();
        }

        private string[] PrepareNamespaces(IEnumerable<string> namespaces, string currentNamespace)
        {
            return namespaces
                .Distinct()
                .Where(n => n != currentNamespace)
                .OrderBy(n => n)
                .ToArray();
        }
    }
}
