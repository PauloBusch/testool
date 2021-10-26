using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Services.Endpoints
{
    public class PostEndpointTestService : EndpointTestServiceBase, IPostEndpointTestService
    {
        public PostEndpointTestService(ICompareService compareService)
            : base(RequestMethodEnumerator.POST, compareService) { }

        public ControllerTestMethod GetControllerTestMethod(Endpoint endpoint, DbSet dbSet)
        {
            var type = TestMethodEnumerator.CREATE;
            var testMethod = new ControllerTestMethod(
                type.Name, endpoint.Method,
                GetArrageSection(endpoint),
                GetActSection(endpoint),
                GetAssertSection(endpoint, dbSet)
            );
            var requiredNamespaces = GetRequitedNamespaces(testMethod, endpoint);
            testMethod.AddRequiredNamespaces(requiredNamespaces);
            return testMethod;
        }
    }
}
