using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Interfaces.Services
{
    public interface IIntegrationTestScanInfraService
    {
        string GetNamespace();
        Task<bool> ProjectExistAsync();
        Task<bool> ClassExistAsync(string className);
        Task<Class> GetClassAsync(string className);
        Task<string> GetPathClassAsync(string className);
        Task<string> MergeClassCodeAsync(string className, string sourceCode);
    }
}
