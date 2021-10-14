using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface IExpressionInfraService
    {
        Task<string> BuildBogusExpressionAsync(string expression);
    }
}
