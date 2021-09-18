using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface IArgumentsService
    {
        Task<ICommand> GetCommandAsync(string[] args);
    }
}
