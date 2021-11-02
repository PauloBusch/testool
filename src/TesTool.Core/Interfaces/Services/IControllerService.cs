using System.Collections.Generic;
using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;

namespace TesTool.Core.Interfaces.Services
{
    public interface IControllerService
    {
        string GetNamespace();
        string GetDirectoryBase();
        string GetControllerTestName(string controller);
        string GetEntityName(string controller, string @default = default); 
        Task<DbSet> GetDbSetClassAsync(string dbContext, string entityName);
        Task<ControllerTest> GetControllerTestAsync(Controller controller, DbSet dbSet);
        Task<IEnumerable<ICommand>> GetRequiredCommandsAsync(ControllerTest controllerTest, bool @static);
    }
}
