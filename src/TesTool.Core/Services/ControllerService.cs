using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Commands.Generate;
using TesTool.Core.Commands.Generate.Fakers;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Interfaces.Services.Fakers;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Services
{
    public class ControllerService : IControllerService
    {
        private readonly IServiceResolver _serviceResolver;
        private readonly ISolutionService _solutionService;
        private readonly IFakeModelService _fakeModelService;
        private readonly IFakeEntityService _fakeEntityService;
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly IPostEndpointTestService _postEndpointTestService;
        private readonly IPutEndpointTestService _putEndpointTestService;
        private readonly IWebApiDbContextInfraService _webApiDbContextInfraService;

        public ControllerService(
            IServiceResolver serviceResolver,
            ISolutionService solutionService,
            IFakeModelService fakeModelService,
            IFakeEntityService fakeEntityService,
            ITestScanInfraService testScanInfraService,
            IPostEndpointTestService postEndpointTestService,
            IPutEndpointTestService putEndpointTestService,
            IWebApiDbContextInfraService webApiDbContextInfraService
        )
        {
            _serviceResolver = serviceResolver;
            _solutionService = solutionService;
            _fakeModelService = fakeModelService;
            _fakeEntityService = fakeEntityService;
            _testScanInfraService = testScanInfraService;
            _postEndpointTestService = postEndpointTestService;
            _putEndpointTestService = putEndpointTestService;
            _webApiDbContextInfraService = webApiDbContextInfraService;
        }

        public string GetControllerName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return default;
            return raw.Contains("Controller") ? raw : $"{raw}Controller";
        }

        public async Task<ControllerTest> GetControllerTestAsync(Controller controller, DbSet dbSet)
        {
            var fixtureName = _solutionService.GetTestFixtureClassName();
            var fixtureClass = await _testScanInfraService.GetClassAsync(fixtureName);
            var testBaseClass = await _testScanInfraService.GetClassAsync(HelpClassEnumerator.TEST_BASE.Name);
            var templateModel = new ControllerTest(
                name: GetControllerTestName(controller.Name), 
                baseRoute: controller.Route, 
                @namespace: GetNamespace(),
                fixtureClass, testBaseClass
            );

            foreach (var endpoint in controller.Endpoints)
            {
                if (endpoint.Method == HttpMethodEnumerator.POST)
                    templateModel.AddMethod(_postEndpointTestService.GetControllerTestMethod(endpoint, dbSet));
                if (endpoint.Method == HttpMethodEnumerator.PUT)
                    templateModel.AddMethod(_putEndpointTestService.GetControllerTestMethod(endpoint, dbSet));
            }

            templateModel.RenameDuplicatedMethods();
            return templateModel;
        }

        public async Task<IEnumerable<ICommand>> GetRequiredCommandsAsync(ControllerTest controllerTest, bool @static)
        {
            var commands = new List<ICommand>();

            var entities = controllerTest.Methods.SelectMany(e => e.Arrage.Entities).Distinct().ToArray();
            foreach (var entity in entities)
            {
                var entityFakerName = _fakeEntityService.GetFakerName(entity);
                if (!await _testScanInfraService.ClassExistAsync(entityFakerName))
                {
                    var generateFakeEntityCommand = _serviceResolver.ResolveService<GenerateFakeEntityCommand>();
                    generateFakeEntityCommand.ClassName = entity;
                    generateFakeEntityCommand.Static = @static;
                    commands.Add(generateFakeEntityCommand);
                }
            }

            var models = controllerTest.Methods.SelectMany(e => e.Arrage.Models).Distinct().ToArray();
            foreach (var model in models)
            {
                var modelFakerName = _fakeModelService.GetFakerName(model);
                if (!await _testScanInfraService.ClassExistAsync(modelFakerName))
                {
                    var generateFakeEntityCommand = _serviceResolver.ResolveService<GenerateFakeModelCommand>();
                    generateFakeEntityCommand.ClassName = model;
                    generateFakeEntityCommand.Static = @static;
                    commands.Add(generateFakeEntityCommand);
                }
            }

            var comparators = controllerTest.Methods
                .SelectMany(e => new [] { e.Assert.ComparatorModel, e.Assert.ComparatorEntity })
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .ToArray();
            foreach (var comparator in comparators)
            {
                if (!await _testScanInfraService.ClassExistAsync(comparator))
                {
                    var classes = comparator.Split("Equals");
                    var generateComparatorCommand = _serviceResolver.ResolveService<GenerateCompareCommand>();
                    generateComparatorCommand.SourceClassName = classes.ElementAt(0);
                    generateComparatorCommand.TargetClassName = classes.ElementAt(1);
                    generateComparatorCommand.Static = @static;
                    commands.Add(generateComparatorCommand);
                }
            }

            return commands;
        }

        public string GetControllerTestName(string controller)
        {
            return $"{controller}Tests";
        }

        public async Task<DbSet> GetDbSetClassAsync(string dbContext, string entityName)
        {
            var classes = await _webApiDbContextInfraService.GetDbSetsAsync(dbContext);
            return classes.FirstOrDefault(c => c.Entity.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase) ||
                c.Property.Equals(entityName, StringComparison.OrdinalIgnoreCase)
            );
        }

        public string GetEntityName(string controller, string @default = null)
        {
            if (!string.IsNullOrWhiteSpace(@default)) return @default;
            return controller.Replace("Controller", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestNamespace("Controllers");
        }

        public string GetDirectoryBase()
        {
            return $"{_testScanInfraService.GetDirectoryBase()}/Controllers";
        }
    }
}
