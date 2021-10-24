using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Commands.Generate.Fakers;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Interfaces.Services.Fakers;
using TesTool.Core.Models.Metadata;
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
        private readonly IEndpointTestFactoryService _endpointFactoryService;
        private readonly IWebApiDbContextInfraService _webApiDbContextInfraService;

        public ControllerService(
            IServiceResolver serviceResolver,
            ISolutionService solutionService,
            IFakeModelService fakeModelService,
            IFakeEntityService fakeEntityService,
            ITestScanInfraService testScanInfraService,
            IEndpointTestFactoryService endpointFactoryService, 
            IWebApiDbContextInfraService webApiDbContextInfraService
        )
        {
            _serviceResolver = serviceResolver;
            _solutionService = solutionService;
            _fakeModelService = fakeModelService;
            _fakeEntityService = fakeEntityService;
            _testScanInfraService = testScanInfraService;
            _endpointFactoryService = endpointFactoryService;
            _webApiDbContextInfraService = webApiDbContextInfraService;
        }

        public string GetControllerName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return default;
            return raw.Contains("Controller") ? raw : $"{raw}Controller";
        }

        public async Task<ControllerTest> GetControllerTestAsync(Controller controller, Class entity)
        {
            var fixtureName = _solutionService.GetTestFixtureClassName();
            var fixtureClass = await _testScanInfraService.GetClassAsync(fixtureName);
            var testBaseClass = await _testScanInfraService.GetClassAsync(TestClassEnumerator.TEST_BASE.Name);
            var templateModel = new ControllerTest(
                name: GetControllerTestName(controller.Name), 
                baseRoute: controller.Route, 
                @namespace: GetNamespace(),
                fixtureClass, testBaseClass
            );

            foreach (var endpoint in controller.Endpoints)
            {
                var testMethod = await _endpointFactoryService.GetControllerTestMethodAsync(controller, endpoint, entity);
                if (testMethod is null) continue;
                templateModel.AddMethod(testMethod);
            }

            return templateModel;
        }

        public async Task<IEnumerable<ICommand>> GetRequiredCommandsAsync(ControllerTest controllerTest, bool @static)
        {
            var commands = new List<ICommand>();

            var entities = controllerTest.Methods.SelectMany(e => e.Entities).ToArray();
            foreach (var entity in entities)
            {
                var entityFakerName = _fakeEntityService.GetFakerName(entity.Name);
                if (!await _testScanInfraService.ClassExistAsync(entityFakerName))
                {
                    var generateFakeEntityCommand = _serviceResolver.ResolveService<GenerateFakeEntityCommand>();
                    generateFakeEntityCommand.ClassName = entity.Name;
                    generateFakeEntityCommand.Static = @static;
                    commands.Add(generateFakeEntityCommand);
                }
            }

            var models = controllerTest.Methods.SelectMany(e => e.Models).ToArray();
            foreach (var model in models)
            {
                var modelFakerName = _fakeModelService.GetFakerName(model.Name);
                if (!await _testScanInfraService.ClassExistAsync(modelFakerName))
                {
                    var generateFakeEntityCommand = _serviceResolver.ResolveService<GenerateFakeModelCommand>();
                    generateFakeEntityCommand.ClassName = model.Name;
                    generateFakeEntityCommand.Static = @static;
                    commands.Add(generateFakeEntityCommand);
                }
            }

            return commands;
        }

        public string GetControllerTestName(string controller)
        {
            return $"{controller}Tests";
        }

        public async Task<Class> GetDbSetClassAsync(string dbContext, string entityName)
        {
            var classes = await _webApiDbContextInfraService.GetDbSetClassesAsync(dbContext);
            return classes.FirstOrDefault(c => c.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
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
