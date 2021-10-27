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
    public class DeleteEndpointTestService : EndpointTestServiceBase, IDeleteEndpointTestService
    {
        public DeleteEndpointTestService(ICompareService compareService) 
            : base(RequestMethodEnumerator.DELETE, compareService) { }

        public ControllerTestMethod GetControllerTestMethod(Controller controller, Endpoint endpoint, DbSet dbSet)
        {
            var type = TestMethodEnumerator.DELETE;
            var testMethod = new ControllerTestMethod(
                type.Name, endpoint.Method,
                GetArrageSection(endpoint, dbSet),
                GetActSection(controller, endpoint, dbSet),
                GetAssertSection(endpoint, dbSet)
            );
            var requiredNamespaces = GetRequitedNamespaces(testMethod, endpoint);
            testMethod.AddRequiredNamespaces(requiredNamespaces);
            testMethod.AddRequiredNamespace("Microsoft.EntityFrameworkCore");
            return testMethod;
        }

        protected override ControllerTestMethodSectionAssertBase GetAssertSection(Endpoint endpoint, DbSet dbSet)
        {
            var entityKey = GetEntityKey(dbSet.Entity);
            return new ControllerTestMethodSectionAssertDelete(
                endpoint.Output is TypeBase type && type.Name != "Void",
                endpoint.Output is Class output && output.Generics.Any(),
                GetPropertyData(endpoint.Output), dbSet.Entity.Name,
                entityKey, dbSet.Property
            );
        }
    }
}
