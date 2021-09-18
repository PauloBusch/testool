using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface ICommandFactoryService
    {
        Task<ICommand> CreateCommandAsync(string[] args);
    }
}
