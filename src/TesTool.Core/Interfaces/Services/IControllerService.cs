using System.Collections.Generic;
using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Interfaces.Services
{
    public interface IControllerService
    {
        string GetNamespace();
        string GetDirectoryBase();
        string GetControllerName(string raw);
        string GetControllerTestName(string controller);
        string GetEntityName(string controller, string @default = default); 
        Task<Class> GetDbSetClassAsync(string dbContext, string entityName);
        Task<ControllerTest> GetControllerTestAsync(Controller controller, Class entity);
        Task<IEnumerable<ICommand>> GetRequiredCommandsAsync(ControllerTest controllerTest, bool @static);
    }
}
