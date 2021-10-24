using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Models.Metadata
{
    public class CommandContext : ICommandContext
    {
        public CommandContext(bool executionCascade)
        {
            ExecutionCascade = executionCascade;
        }

        public bool ExecutionCascade { get; private set; }
    }
}
