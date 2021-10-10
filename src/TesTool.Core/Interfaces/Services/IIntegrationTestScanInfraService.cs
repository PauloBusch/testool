using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Interfaces.Services
{
    public interface IIntegrationTestScanInfraService
    {
        string GetNamespace();
        Task<bool> ProjectExistAsync();
        Task<Class> GetClassAsync(string name);
    }
}
