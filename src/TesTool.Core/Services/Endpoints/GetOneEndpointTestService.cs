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
    public class GetOneEndpointTestService : EndpointTestServiceBase, IGetOneEndpointTestService
    {
        public GetOneEndpointTestService(ICompareService compareService)
            : base(RequestMethodEnumerator.GET, compareService) { }

        public ControllerTestMethod GetControllerTestMethod(Controller controller, Endpoint endpoint, DbSet dbSet)
        {
            var assert = GetAssertSection(endpoint, dbSet) as ControllerTestMethodSectionAssertGetOne;
            var testMethod = new ControllerTestMethod(
                GetMethodName(endpoint, dbSet), endpoint.Method,
                GetArrageSection(endpoint, dbSet),
                GetActSection(controller, endpoint, dbSet),
                assert
            );
            var requiredNamespaces = GetRequitedNamespaces(testMethod, endpoint);
            testMethod.AddRequiredNamespaces(requiredNamespaces);
            
            var entityVariable = dbSet?.Entity.Name.ToLowerCaseFirst();
            if (string.IsNullOrWhiteSpace(assert.ComparatorEntity) && dbSet is not null && 
                testMethod.Act.Route is not null && !testMethod.Act.Route.Contains(entityVariable)
            )
                testMethod.Arrage.RemoveEntity(dbSet?.Entity.Name);

            return testMethod;
        }

        protected override ControllerTestMethodSectionAssertBase GetAssertSection(Endpoint endpoint, DbSet dbSet)
        {
            var responseModel = GetOutputModel(endpoint.Output);
            return new ControllerTestMethodSectionAssertGetOne(
                endpoint.Output is TypeBase type && type.Name != "Void",
                endpoint.Output is Class output && output.Generics.Any(),
                GetPropertyData(endpoint.Output), dbSet?.Entity.Name,
                _compareService.IsComparableClasses(dbSet?.Entity, responseModel as Class) 
                    ? _compareService.GetComparatorNameOrDefault(dbSet?.Entity.Name, responseModel?.Name) : default
            );
        }

        private string GetMethodName(Endpoint endpoint, DbSet dbSet)
        {
            var type = TestMethodEnumerator.RETURN;
            var entityKey = GetEntityKey(dbSet?.Entity);
            var hasOutput = endpoint.Output is TypeBase typeBase && typeBase.Name != "Void";
            if (hasOutput && endpoint.Route is not null && !string.IsNullOrWhiteSpace(entityKey) && 
                endpoint.Route.Contains(entityKey, StringComparison.OrdinalIgnoreCase)
            )
                return type.Name.Replace("{ARTIFACT}", $"By{entityKey}");
            if (hasOutput && GetOutputModel(endpoint.Output) is Class @class)
                return type.Name.Replace("{ARTIFACT}", @class.Name);

            var action = endpoint.Name.Replace("Async", string.Empty, StringComparison.OrdinalIgnoreCase);
            return TestMethodEnumerator.GENERIC.Name.Replace("{ACTION}", action, StringComparison.OrdinalIgnoreCase);
        }
    }
}
