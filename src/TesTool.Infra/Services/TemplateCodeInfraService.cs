using System.Collections.Generic;
using System.Linq;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Templates.Comparator;
using TesTool.Core.Models.Templates.Controller;
using TesTool.Core.Models.Templates.Factories;
using TesTool.Core.Models.Templates.Faker;
using TesTool.Infra.Templates.Comparators;
using TesTool.Infra.Templates.Controller;
using TesTool.Infra.Templates.Extensions;
using TesTool.Infra.Templates.Factories;
using TesTool.Infra.Templates.Fakers;
using TesTool.Infra.Templates.Helpers;

namespace TesTool.Infra.Services
{
    public class TemplateCodeInfraService : ITemplateCodeInfraService
    {
        public string BuildControllerTest(ControllerTest model)
        {
            var template = new ControllerTestTemplate
            {
                Name = model.Name,
                TemplataService = this,
                Namespace = model.Namespace,
                BaseRoute = model.BaseRoute,
                FixtureName = model.FixtureName,
                Namespaces = PrepareNamespaces(model.Namespaces, model.Namespace),
                Methods = model.Methods.ToArray()
            };
            return template.TransformText();
        }

        public string BuildControllerTestMethod(ControllerTestMethod model)
        {
            var template = new ControllerTestMethodTemplate
            {
                Name = model.Name,
                TemplataService = this,
                Unsafe = model.Unsafe,
                Method = model.Method,
                Arrage = model.Arrage,
                Act = model.Act,
                Assert = model.Assert
            };
            return template.TransformText();
        }

        public string BuildControllerTestMethodSectionArrage(ControllerTestMethodSectionArrage model)
        {
            var template = new ControllerTestMethodSectionArrageTemplate
            {
                Entities = model.Entities.ToArray(),
                Models = model.Models.ToArray()
            };
            return template.TransformText();
        }

        public string BuildControllerTestMethodSectionAct(ControllerTestMethodSectionAct model)
        {
            var template = new ControllerTestMethodSectionActTemplate
            {
                Route = model.Route,
                Method = model.Method,
                ReturnType = model.ReturnType,
                BodyModel = model.BodyModel,
                QueryModel = model.QueryModel
            };
            return template.TransformText();
        }

        public string BuildControllerTestMethodSectionAssertPost(ControllerTestMethodSectionAssert model)
        {
            var template = new ControllerTestMethodSectionAssertPostTemplate
            {
                HaveOutput = model.HaveOutput,
                RequestHaveKey = model.RequestHaveKey,
                ResponseHaveKey = model.ResponseHaveKey,
                ResponseIsGeneric = model.ResponseIsGeneric,
                EntityKey = model.EntityKey,
                EntityDbSet = model.EntityDbSet,
                PropertyData = model.PropertyData,
                EntityName = model.EntityName,
                RequestModel = model.RequestModel,
                ComparatorModel = model.ComparatorModel,
                ComparatorEntity = model.ComparatorEntity

            };
            return template.TransformText();
        }

        public string BuildModelFaker(ModelFaker model)
        {
            var template = new ModelFakerTemplate
            {
                Name = model.Name,
                FakerNamespace = model.FakerNamespace,
                Namespaces = PrepareNamespaces(model.Namespaces, model.FakerNamespace),
                Properties = model.Properties.ToArray()
            };
            return template.TransformText();
        }

        public string BuildModelFakerFactory(ModelFakerFactory model)
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

        public string BuildModelFakerFactoryMethod(ModelFakerFactoryMethod model)
        {
            var template = new ModelFakerFactoryMethodTemplate { Method = model };
            return template.TransformText();
        }

        public string BuildEntityFaker(EntityFaker model)
        {
            var template = new EntityFakerTemplate
            {
                Name = model.Name,
                DbContext = model.DbContext,
                FakerNamespace = model.FakerNamespace,
                Namespaces = PrepareNamespaces(model.Namespaces, model.FakerNamespace),
                Properties = model.Properties.ToArray()
            };
            return template.TransformText();
        }

        public string BuildEntityFakerFactory(EntityFakerFactory model)
        {
            var template = new EntityFakerFactoryTemplate
            {
                TemplataService = this,
                Name = model.Name,
                TestBase = model.TestBase,
                DbContext = model.DbContext,
                Methods = model.Methods.ToArray(),
                Namespaces = PrepareNamespaces(model.Namespaces, model.FactoryNamespace),
                FactoryNamespace = model.FactoryNamespace,
            };
            return template.TransformText();
        }

        public string BuildEntityFakerFactoryMethod(EntityFakerFactoryMethod model)
        {
            var template = new EntityFakerFactoryMethodTemplate { Method = model };
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
