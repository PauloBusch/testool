using System.Collections.Generic;
using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface ICommandHandler
    {
        Task HandleAsync(ICommand command, bool cascade = true);
        Task HandleManyAsync(IEnumerable<ICommand> commands, bool cascade = true);
    }
}
