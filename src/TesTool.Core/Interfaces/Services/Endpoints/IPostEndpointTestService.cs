using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Interfaces.Services.Endpoints
{
    public interface IPostEndpointTestService 
    {
        ControllerTestMethod GetControllerTestMethod(Endpoint endpoint, DbSet dbSet);
    }
}
