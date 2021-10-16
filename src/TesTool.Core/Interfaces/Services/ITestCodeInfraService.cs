using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface ITestCodeInfraService
    {
        Task<string> MergeClassCodeAsync(string className, string sourceCode);
    }
}
