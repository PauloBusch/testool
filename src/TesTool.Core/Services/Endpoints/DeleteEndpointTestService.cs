using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Services.Endpoints
{
    public class DeleteEndpointTestService : EndpointTestServiceBase, IDeleteEndpointTestService
    {
        public DeleteEndpointTestService(ICompareService compareService) 
            : base(RequestMethodEnumerator.DELETE, compareService) { }

        public ControllerTestMethod GetControllerTestMethod(Endpoint endpoint, DbSet dbSet)
        {
            var type = TestMethodEnumerator.DELETE;
            var testMethod = new ControllerTestMethod(
                type.Name, endpoint.Method,
                GetArrageSection(endpoint, dbSet),
                GetActSection(endpoint, dbSet),
                GetAssertSection(endpoint, dbSet)
            );
            var requiredNamespaces = GetRequitedNamespaces(testMethod, endpoint);
            testMethod.AddRequiredNamespaces(requiredNamespaces);
            testMethod.AddRequiredNamespace("Microsoft.EntityFrameworkCore");
            return testMethod;
        }
    }
}
