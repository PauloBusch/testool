using System;
using System.Linq;
using TesTool.Core.Enumerations;
using TesTool.Core.Extensions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;
using TesTool.Core.Models.Templates.Controller.Asserts;

namespace TesTool.Core.Services.Endpoints
{
    public class GetListEndpointTestService : EndpointTestServiceBase, IGetListEndpointTestService
    {
        public GetListEndpointTestService(ICompareService compareService)
            : base(RequestMethodEnumerator.GET, compareService) { }

        public ControllerTestMethod GetControllerTestMethod(Controller controller, Endpoint endpoint, DbSet dbSet)
        {
            var assert = GetAssertSection(endpoint, dbSet) as ControllerTestMethodSectionAssertGetList;
            var testMethod = new ControllerTestMethod(
                GetMethodName(endpoint, dbSet), endpoint.Method,
                GetArrageSection(endpoint, dbSet),
                GetActSection(controller, endpoint, dbSet),
                assert
            );
            var requiredNamespaces = GetRequitedNamespaces(testMethod, endpoint);
            testMethod.AddRequiredNamespaces(requiredNamespaces);
            testMethod.AddRequiredNamespace("System.Linq");

            var entityVariable = dbSet?.Entity.Name.ToLowerCaseFirst();
            if (string.IsNullOrWhiteSpace(assert.ComparatorEntity) && dbSet is not null && 
                testMethod.Act.Route is not null && !testMethod.Act.Route.Contains(entityVariable)
            )
                testMethod.Arrage.RemoveEntity(dbSet.Entity.Name);
            return testMethod;
        }

        protected override ControllerTestMethodSectionAssertBase GetAssertSection(Endpoint endpoint, DbSet dbSet)
        {
            var entityKey = GetEntityKey(dbSet?.Entity);
            var arrayModel = GetOutputModel(endpoint.Output) as Models.Metadata.Array;
            var responseModel = arrayModel.Type as TypeBase;
            return new ControllerTestMethodSectionAssertGetList(
                true, endpoint.Output is Class output && output.Generics.Any(),
                (responseModel as Class)?.Properties.Any(p => p.Name == entityKey) ?? false,
                GetPropertyData(endpoint.Output), dbSet?.Entity.Name, entityKey,
                _compareService.GetComparatorNameOrDefault(dbSet?.Entity.Name, responseModel?.Name)
            );
        }

        private string GetMethodName(Endpoint endpoint, DbSet dbSet)
        {
            var type = TestMethodEnumerator.RETURN;
            var arrayModel = GetOutputModel(endpoint.Output) as Models.Metadata.Array;
            var responseModel = arrayModel.Type as TypeBase;
            if (responseModel is not null)
            {
                if (dbSet is not null && responseModel.Name.Contains(dbSet.Entity.Name))
                    return type.Name.Replace("{ARTIFACT}", "Many");
                return type.Name.Replace("{ARTIFACT}", $"{responseModel.Name}Many");
            }

            var action = endpoint.Name.Replace("Async", string.Empty, StringComparison.OrdinalIgnoreCase);
            return TestMethodEnumerator.GENERIC.Name.Replace("{ACTION}", action, StringComparison.OrdinalIgnoreCase);
        }
    }
}
