using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Services.Endpoints
{
    public class PutEndpointTestService : EndpointTestServiceBase, IPutEndpointTestService
    {
        public PutEndpointTestService(ICompareService compareService)
            : base(RequestMethodEnumerator.PUT, compareService) { }

        public ControllerTestMethod GetControllerTestMethod(Endpoint endpoint, DbSet dbSet)
        {
            var type = TestMethodEnumerator.UPDATE;
            var testMethod = new ControllerTestMethod(
                type.Name, endpoint.Method,
                GetArrageSection(endpoint, dbSet),
                GetActSection(endpoint, dbSet),
                GetAssertSection(endpoint, dbSet)
            );
            testMethod.AddRequiredNamespaces(GetOutputNamespaces(endpoint.Output));
            return testMethod;
        }
    }
}
