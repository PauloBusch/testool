﻿using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Fakers;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Services.Fakers
{
    public class FakeEntityService : FakeServiceBase, IFakeEntityService
    {
        private readonly ISettingInfraService _settingInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;

        public FakeEntityService(
            ISolutionInfraService solutionService,
            IWebApiScanInfraService webApiScanInfraService,
            ISettingInfraService settingInfraService,
            ITestScanInfraService testScanInfraService,
            IExpressionInfraService expressionInfraService,
            IConventionInfraService conventionInfraService
        ) : base(
            solutionService, testScanInfraService,
            expressionInfraService, conventionInfraService
        )
        { 
            _webApiScanInfraService = webApiScanInfraService;
            _settingInfraService = settingInfraService;    
        }

        public string GetDirectoryBase()
        {
            return $"{_testScanInfraService.GetDirectoryBase()}/Fakers/Entities";
        }

        public async Task<EntityFaker> GetFakerEntityAsync(Class model, bool @static)
        {
            var dbContextClass = await _webApiScanInfraService.GetModelAsync(_settingInfraService.DbContextName) as Class;
            var entityFakerBase = await _testScanInfraService.GetClassAsync(HelpClassEnumerator.ENTITY_FAKER_BASE.Name);
            var templateModel = new EntityFaker(model.Name, GetNamespace(), entityFakerBase, dbContextClass);
            return await FillTemplateAsync<EntityFaker, EntityProperty>(templateModel, model, BogusMethodEnumerator.ENTITY_OBJECT, @static);
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestProjectNamespace("Fakers.Entities");
        }

        protected override T MapProperty<T>(string name, string expression, bool @unsafe)
        {
            return new EntityProperty(name, expression, @unsafe) as T;
        }
    }
}
