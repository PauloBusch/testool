using System.Collections.Generic;
using System.Linq;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Templates.Comparator;
using TesTool.Core.Models.Templates.Factory;
using TesTool.Infra.Templates;
using TesTool.Infra.Templates.Comparators;
using TesTool.Infra.Templates.Extensions;
using TesTool.Infra.Templates.Factories;
using TesTool.Infra.Templates.Fakers;
using TesTool.Infra.Templates.Helpers;

namespace TesTool.Infra.Services
{
    public class TemplateCodeInfraService : ITemplateCodeInfraService
    {
        public string BuildModel(Core.Models.Templates.Faker.Bogus model)
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

        public string BuildModelFactory(ModelFactory model)
        {
            var template = new ModelFactoryTemplate
            { 
                TemplataService = this,
                Name = model.Name,
                Methods = model.Methods.ToArray(),
                Namespaces = PrepareNamespaces(model.Namespaces, model.FactoryNamespace),
                FactoryNamespace = model.FactoryNamespace,
            };
            return template.TransformText();
        }

        public string BuildModelFactoryMethod(ModelFactoryMethod model)
        {
            var template = new ModelFactoryMethodTemplate { Method = model };
            return template.TransformText();
        }

        public string BuildCompareStatic(CompareStatic model)
        {
            var template = new ComparatorStaticTemplate
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

        public string BuildCompareDynamic(CompareDynamic model)
        {
            var template = new ComparatorDynamicTemplate
            {
                ComparatorNamespace = model.ComparatorNamespace,
                ComparatorClassName = model.ComparatorClassName,
                SourceClassName = model.SourceClassName,
                TargetClassName = model.TargetClassName,
                Namespaces = PrepareNamespaces(model.Namespaces, model.ComparatorNamespace)
            };
            return template.TransformText();
        }

        public string BuildComparatorFactory(ComparatorFactory model)
        {
            var template = new ComparatorFactoryTemplate
            {
                TemplataService = this,
                Name = model.Name,
                Methods = model.Methods.ToArray(),
                Namespaces = PrepareNamespaces(model.Namespaces, model.FactoryNamespace),
                FactoryNamespace = model.FactoryNamespace,
            };
            return template.TransformText();
        }

        public string BuildComparatorFactoryMethod(ComparatorFactoryMethod model)
        {
            var template = new ComparatorFactoryMethodTemplate { Method = model };
            return template.TransformText();
        }

        public string BuildAssertExtensions(string @namespace)
        {
            var template = new AssertExtensionsTemplate { ExtensionNamespace = @namespace };
            return template.TransformText();
        }

        public string BuildHttpRequest(string @namespace)
        {
            var template = new HttpRequestTemplate { Namespace = @namespace };
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
