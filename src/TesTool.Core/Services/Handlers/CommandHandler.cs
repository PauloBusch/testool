using System.Collections.Generic;
using System.Threading.Tasks;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Services.Handlers
{
    public class CommandHandler : ICommandHandler
    {
        public async Task HandleAsync(ICommand command, bool cascade = true)
        {
            await command.ExecuteAsync(new CommandContext(cascade));
        }

        public async Task HandleManyAsync(IEnumerable<ICommand> commands, bool cascade = true)
        {
            foreach (var command in commands) await command.ExecuteAsync(new CommandContext(cascade));
        }
    }
}
