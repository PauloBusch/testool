using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Interfaces
{
    public interface ICommand
    {
        Task ExecuteAsync(ICommandContext context);
    }
}
