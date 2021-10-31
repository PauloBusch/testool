using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly ISolutionInfraService _solutionService;
        private readonly IFixtureService _fixtureService;
        private readonly IFakeModelService _fakeModelService;
        private readonly IFakeEntityService _fakeEntityService;
        private readonly ITestScanInfraService _testScanInfraService;
        private readonly IWebApiDbContextInfraService _webApiDbContextInfraService;

        private readonly IGetOneEndpointTestService _getOneEndpointTestService;
        private readonly IGetListEndpointTestService _getListEndpointTestService;
        private readonly IPostEndpointTestService _postEndpointTestService;
        private readonly IPutEndpointTestService _putEndpointTestService;
        private readonly IDeleteEndpointTestService _deleteEndpointTestService;

        public ControllerService(
            IFixtureService fixtureService,
            IServiceResolver serviceResolver,
            ISolutionInfraService solutionService,
            IFakeModelService fakeModelService,
            IFakeEntityService fakeEntityService,
            ITestScanInfraService testScanInfraService,
            IWebApiDbContextInfraService webApiDbContextInfraService,
            IGetOneEndpointTestService getOneEndpointTestService,
            IGetListEndpointTestService getListEndpointTestService,
            IPostEndpointTestService postEndpointTestService,
            IPutEndpointTestService putEndpointTestService,
            IDeleteEndpointTestService deleteEndpointTestService
        )
        {
            _fixtureService = fixtureService;
            _serviceResolver = serviceResolver;
            _solutionService = solutionService;
            _fakeModelService = fakeModelService;
            _fakeEntityService = fakeEntityService;
            _testScanInfraService = testScanInfraService;
            _webApiDbContextInfraService = webApiDbContextInfraService;
            
            _getOneEndpointTestService = getOneEndpointTestService;
            _getListEndpointTestService = getListEndpointTestService;
            _postEndpointTestService = postEndpointTestService;
            _putEndpointTestService = putEndpointTestService;
            _deleteEndpointTestService = deleteEndpointTestService;
        }

        public string GetControllerName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return default;
            return raw.Contains("Controller") ? raw : $"{raw}Controller";
        }

        public async Task<ControllerTest> GetControllerTestAsync(Controller controller, DbSet dbSet)
        {
            var fixtureName = _fixtureService.GetFixtureName();
            var fixtureClass = await _testScanInfraService.GetClassAsync(fixtureName);
            var testBaseClass = await _testScanInfraService.GetClassAsync(HelpClassEnumerator.TEST_BASE.Name);
            var templateModel = new ControllerTest(
                name: GetControllerTestName(controller.Name), 
                baseRoute: GetBaseRoute(controller.Route), 
                @namespace: GetNamespace(),
                fixtureClass, testBaseClass
            );

            foreach (var endpoint in controller.Endpoints)
            {
                if (endpoint.Method == HttpMethodEnumerator.POST)
                    templateModel.AddMethod(_postEndpointTestService.GetControllerTestMethod(controller, endpoint, dbSet));
                if (endpoint.Method == HttpMethodEnumerator.PUT)
                    templateModel.AddMethod(_putEndpointTestService.GetControllerTestMethod(controller, endpoint, dbSet));
                if (endpoint.Method == HttpMethodEnumerator.DELETE)
                    templateModel.AddMethod(_deleteEndpointTestService.GetControllerTestMethod(controller, endpoint, dbSet));
                if (endpoint.Method == HttpMethodEnumerator.GET)
                {
                    var returnVoid = endpoint.Output is TypeBase type && type.Name == "Void";
                    if (returnVoid) templateModel.AddMethod(_getOneEndpointTestService.GetControllerTestMethod(controller, endpoint, dbSet));
                    else
                    {
                        var output = GetOutputModel(endpoint.Output);
                        if (output is Models.Metadata.Array)
                            templateModel.AddMethod(_getListEndpointTestService.GetControllerTestMethod(controller, endpoint, dbSet));
                        else templateModel.AddMethod(_getOneEndpointTestService.GetControllerTestMethod(controller, endpoint, dbSet));
                    }
                }
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
                .SelectMany(e => e.Assert.GetComparators())
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
            return _solutionService.GetTestProjectNamespace("Controllers");
        }

        public string GetDirectoryBase()
        {
            return $"{_testScanInfraService.GetDirectoryBase()}/Controllers";
        }

        private string GetBaseRoute(string route)
        {
            var match = Regex.Match(route, "{(.*?)}");
            if (!match.Success || match.Groups.Count < 2) return route;

            return route.Substring(0, match.Index).Trim('/');
        }

        private string GetEntityKey(Class entity)
        {
            if (entity is null) return default;

            var expectedKey = "Id";
            var propertyKey = entity.Properties.FirstOrDefault(p => p.Name.Equals(expectedKey, StringComparison.OrdinalIgnoreCase));
            propertyKey ??= entity.Properties.FirstOrDefault(p => p.Name.EndsWith(expectedKey, StringComparison.OrdinalIgnoreCase));
            return propertyKey?.Name;
        }

        private TypeBase GetOutputModel(TypeWrapper wrapper)
        {
            if (wrapper is Class @class)
            {
                if (!@class.Generics.Any()) return @class;

                var genericProperty = @class.Properties.FirstOrDefault(p => p.FromGeneric);
                if (genericProperty is null) return @class;

                return GetOutputModel(genericProperty.Type);
            }

            if (wrapper is Models.Metadata.Array array) 
                return array;

            return default;
        }
    }
}
