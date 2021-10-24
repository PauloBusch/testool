using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Services.Endpoints
{
    public class PostEndpointTestService : EndpointTestServiceBase, IPostEndpointTestService
    {
        public PostEndpointTestService() : base(RequestMethodEnumerator.POST) { }

        public ControllerTestMethod GetControllerTestMethod(Endpoint endpoint, Class entity)
        {
            var testMethod = new ControllerTestMethod("ShouldCreateAsync", GetRequestCall(endpoint));
            testMethod.AddModels(GetInputModels(endpoint.Inputs));
            return testMethod;
        }
    }
}
