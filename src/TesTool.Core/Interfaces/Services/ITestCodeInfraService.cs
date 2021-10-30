using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface ITestCodeInfraService
    {
        Task CreateTestProjectAsync(string name, string output);
        Task<string> MergeClassCodeAsync(string className, string sourceCode);
    }
}
