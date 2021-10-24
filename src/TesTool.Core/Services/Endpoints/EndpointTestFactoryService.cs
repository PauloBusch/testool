using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Services.Endpoints
{
    public class EndpointTestFactoryService : IEndpointTestFactoryService
    {
        private readonly IPostEndpointTestService _postEndpointTestService;

        public EndpointTestFactoryService(IPostEndpointTestService postEndpointTestService)
        {
            _postEndpointTestService = postEndpointTestService;
        }

        public async Task<ControllerTestMethod> GetControllerTestMethodAsync(Controller controller, Endpoint endpoint, Class entity)
        {
            if (endpoint.Method == HttpMethodEnumerator.POST) return _postEndpointTestService.GetControllerTestMethod(endpoint, entity);
            
            return default;
        }
    }
}
