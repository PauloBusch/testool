using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Interfaces.Services.Endpoints
{
    public interface IEndpointTestFactoryService
    {
        Task<ControllerTestMethod> GetControllerTestMethodAsync(Controller controller, Endpoint endpoint, Class entity);
    }
}
