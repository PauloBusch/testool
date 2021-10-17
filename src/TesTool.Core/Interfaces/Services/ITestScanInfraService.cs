using System.Collections.Generic;
using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Interfaces.Services
{
    public interface ITestScanInfraService
    {
        string GetNamespace();
        Task<bool> ProjectExistAsync();
        Task<bool> ClassExistAsync(string className);
        Task<Class> GetClassAsync(string className);
        Task<string> GetPathClassAsync(string className);
    }
}
