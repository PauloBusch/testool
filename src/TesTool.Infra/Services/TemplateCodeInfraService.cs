using System.Collections.Generic;
using System.Linq;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Templates.Common;
using TesTool.Core.Models.Templates.Comparator;
using TesTool.Core.Models.Templates.Controller;
using TesTool.Core.Models.Templates.Controller.Asserts;
using TesTool.Core.Models.Templates.Factories;
using TesTool.Core.Models.Templates.Faker;
using TesTool.Infra.Templates.Common;
using TesTool.Infra.Templates.Common.Extensions;
using TesTool.Infra.Templates.Common.Utils;
using TesTool.Infra.Templates.Comparators;
using TesTool.Infra.Templates.Controllers;
using TesTool.Infra.Templates.Factories;
using TesTool.Infra.Templates.Fakers;

namespace TesTool.Infra.Services
{
    public class TemplateCodeInfraService : ITemplateCodeInfraService
    {
        private readonly ISolutionInfraService _solutionService;

        public TemplateCodeInfraService(ISolutionInfraService solutionService)
        {
            _solutionService = solutionService;
        }

        public string BuildFixture(Fixture model)
        {
            var template = new FixtureTemplate
            {
                DbContext = model.DbContext,
                ProjectName = model.ProjectName,
                FixtureName = model.FixtureName,
                FixtureNamespace = model.FixtureNamespace,
                Namespaces = PrepareNamespaces(model.Namespaces, model.FixtureNamespace)
            };
            return TrimRows(template.TransformText());
        }

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
            return TrimRows(template.TransformText());
        }

        public string BuildControllerTestMethod(ControllerTestMethod model)
        {
            var template = new ControllerTestMethodTemplate
            {
                Name = model.Name,
                TemplataService = this,
                Method = model.Method,
                Arrage = model.Arrage,
                Act = model.Act,
                Assert = model.Assert
            };
            return TrimRows(template.TransformText());
        }

        public string BuildControllerTestMethodSectionArrage(ControllerTestMethodSectionArrage model)
        {
            var template = new ControllerTestMethodSectionArrageTemplate
            {
                Entities = model.Entities.ToArray(),
                Models = model.Models.ToArray(),
                IsEmpty = model.IsEmpty
            };
            return TrimRows(template.TransformText());
        }

        public string BuildControllerTestMethodSectionAct(ControllerTestMethodSectionAct model)
        {
            var template = new ControllerTestMethodSectionActTemplate
            {
                Route = model.Route,
                Method = model.Method,
                Unsafe = model.Unsafe,
                ReturnType = model.ReturnType,
                BodyModel = model.BodyModel,
                QueryModel = model.QueryModel
            };
            return TrimRows(template.TransformText());
        }

        public string BuildControllerTestMethodSectionAssert(ControllerTestMethodSectionAssertBase model)
        {
            if (model is ControllerTestMethodSectionAssertGetOne assertGetOne)
                return BuildControllerTestMethodSectionAssertGetOne(assertGetOne);
            if (model is ControllerTestMethodSectionAssertGetList assertGetList)
                return BuildControllerTestMethodSectionAssertGetList(assertGetList);
            if (model is ControllerTestMethodSectionAssertPost assertPost) 
                return BuildControllerTestMethodSectionAssertPost(assertPost);
            if (model is ControllerTestMethodSectionAssertPut assertPut)
                return BuildControllerTestMethodSectionAssertPut(assertPut);
            if (model is ControllerTestMethodSectionAssertDelete assertDelete)
                return BuildControllerTestMethodSectionAssertDelete(assertDelete);

            return default;
        }

        public string BuildControllerTestMethodSectionAssertGetOne(ControllerTestMethodSectionAssertGetOne model)
        {
            var template = new ControllerTestMethodSectionAssertGetOneTemplate
            {
                HaveOutput = model.HaveOutput,
                ResponseIsGeneric = model.ResponseIsGeneric,
                PropertyData = model.PropertyData,
                EntityName = model.EntityName,
                ComparatorEntity = model.ComparatorEntity
            };
            return TrimRows(template.TransformText());
        }

        public string BuildControllerTestMethodSectionAssertGetList(ControllerTestMethodSectionAssertGetList model)
        {
            var template = new ControllerTestMethodSectionAssertGetListTemplate
            {
                ResponseHaveKey = model.ResponseHaveKey,
                ResponseIsGeneric = model.ResponseIsGeneric,
                PropertyData = model.PropertyData,
                EntityName = model.EntityName,
                ComparatorEntity = model.ComparatorEntity,
                EntityKey = model.EntityKey
            };
            return TrimRows(template.TransformText());
        }

        public string BuildControllerTestMethodSectionAssertPost(ControllerTestMethodSectionAssertPost model)
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
            return TrimRows(template.TransformText());
        }

        public string BuildControllerTestMethodSectionAssertPut(ControllerTestMethodSectionAssertPut model)
        {
            var template = new ControllerTestMethodSectionAssertPutTemplate
            {
                HaveOutput = model.HaveOutput,
                ResponseIsGeneric = model.ResponseIsGeneric,
                PropertyData = model.PropertyData,
                EntityName = model.EntityName,
                RequestModel = model.RequestModel,
                ComparatorModel = model.ComparatorModel,
                ComparatorEntity = model.ComparatorEntity
            };
            return TrimRows(template.TransformText());
        }

        public string BuildControllerTestMethodSectionAssertDelete(ControllerTestMethodSectionAssertDelete model)
        {
            var template = new ControllerTestMethodSectionAssertDeleteTemplate
            {
                HaveOutput = model.HaveOutput,
                ResponseIsGeneric = model.ResponseIsGeneric,
                PropertyData = model.PropertyData,
                EntityName = model.EntityName,
                EntityKey = model.EntityKey,
                EntityDbSet = model.EntityDbSet
            };
            return TrimRows(template.TransformText());
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
            return TrimRows(template.TransformText());
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
            return TrimRows(template.TransformText());
        }

        public string BuildModelFakerFactoryMethod(ModelFakerFactoryMethod model)
        {
            var template = new ModelFakerFactoryMethodTemplate { Method = model };
            return TrimRows(template.TransformText());
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
            return TrimRows(template.TransformText());
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
            return TrimRows(template.TransformText());
        }

        public string BuildEntityFakerFactoryMethod(EntityFakerFactoryMethod model)
        {
            var template = new EntityFakerFactoryMethodTemplate { Method = model };
            return TrimRows(template.TransformText());
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
            return TrimRows(template.TransformText());
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
            return TrimRows(template.TransformText());
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
            return TrimRows(template.TransformText());
        }

        public string BuildComparatorFactoryMethod(ComparatorFactoryMethod model)
        {
            var template = new ComparatorFactoryMethodTemplate { Method = model };
            return TrimRows(template.TransformText());
        }

        public string BuildAssertExtensions(string @namespace)
        {
            var template = new AssertExtensionsTemplate { ExtensionNamespace = @namespace };
            return TrimRows(template.TransformText());
        }

        public string BuildHttpRequest(string @namespace)
        {
            var template = new HttpRequestTemplate { Namespace = @namespace };
            return TrimRows(template.TransformText());
        }

        public string BuildProjectExplorer(string @namespace)
        {
            var template = new ProjectExplorerTemplate { Namespace = @namespace };
            return TrimRows(template.TransformText());
        }

        public string BuildConfigurationLoader(string @namespace)
        {
            var template = new ConfigurationLoaderTemplate { Namespace = @namespace };
            return TrimRows(template.TransformText());
        }

        private string[] PrepareNamespaces(IEnumerable<string> namespaces, string currentNamespace)
        {
            var baseTestNamespace = _solutionService.GetTestNamespace();
            return namespaces
                .Distinct()
                .Where(n => n != currentNamespace)
                .Where(n => n != baseTestNamespace)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .OrderBy(n => n)
                .ToArray();
        }

        private static string TrimRows(string code)
        {
            return code.Trim('\r', '\n');
        }
    }
}
