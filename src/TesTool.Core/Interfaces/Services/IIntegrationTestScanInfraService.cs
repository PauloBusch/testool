using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Interfaces.Services
{
    public interface IIntegrationTestScanInfraService
    {
        string GetNamespace();
        Task<Dto> GetClassAsync(string name);
    }
}
