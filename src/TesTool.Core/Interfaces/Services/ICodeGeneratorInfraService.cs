using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface ICodeGeneratorInfraService
    {
        Task CreateClassAsync(/*data*/);
        Task AppendMethodAsync(/*data*/);
    }
}
