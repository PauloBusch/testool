using System.Linq;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;
using TesTool.Core.Models.Templates.Controller.Asserts;

namespace TesTool.Core.Services.Endpoints
{
    public class PostEndpointTestService : EndpointTestServiceBase, IPostEndpointTestService
    {
        public PostEndpointTestService(ICompareService compareService)
            : base(RequestMethodEnumerator.POST, compareService) { }

        public ControllerTestMethod GetControllerTestMethod(Controller controller, Endpoint endpoint, DbSet dbSet)
        {
            var type = TestMethodEnumerator.CREATE;
            var testMethod = new ControllerTestMethod(
                type.Name, endpoint.Method,
                GetArrageSection(endpoint),
                GetActSection(controller, endpoint),
                GetAssertSection(endpoint, dbSet)
            );
            var requiredNamespaces = GetRequitedNamespaces(testMethod, endpoint);
            testMethod.AddRequiredNamespaces(requiredNamespaces);
            return testMethod;
        }

        protected override ControllerTestMethodSectionAssertBase GetAssertSection(Endpoint endpoint, DbSet dbSet)
        {
            var entityKey = GetEntityKey(dbSet.Entity);
            var requestModel = GetModelComparableEntity(endpoint.Inputs, dbSet.Entity);
            var responseModel = GetOutputModel(endpoint.Output) as Class;
            return new ControllerTestMethodSectionAssertPost(
                requestModel?.Properties.Any(p => p.Name == entityKey) ?? false,
                responseModel?.Properties.Any(p => p.Name == entityKey) ?? false,
                endpoint.Output is TypeBase type && type.Name != "Void",
                endpoint.Output is Class output && output.Generics.Any(),
                GetPropertyData(endpoint.Output), dbSet.Entity.Name, 
                entityKey, dbSet.Property, requestModel?.Name,
                _compareService.GetComparatorNameOrDefault(requestModel?.Name, responseModel?.Name),
                _compareService.GetComparatorNameOrDefault(requestModel?.Name, dbSet.Entity.Name)
            );
        }
    }
}
