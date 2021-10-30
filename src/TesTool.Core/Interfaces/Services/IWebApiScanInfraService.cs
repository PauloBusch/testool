using System.Collections.Generic;
using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Interfaces.Services
{
    public interface IWebApiScanInfraService
    {
        string GetName();
        string GetNamespace();
        string GetProjectPathFile();
        Task<bool> ProjectExistAsync();
        Task<bool> ModelExistAsync(string className);
        Task<TypeWrapper> GetModelAsync(string className);
        Task<Controller> GetControllerAsync(string className);
        Task<IEnumerable<Controller>> GetControllersAsync();
    }
}
