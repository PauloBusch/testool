using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;

namespace TesTool.Core.Commands.Generate
{
    [Command("generate", "g")]
    public abstract class GenerateCommandBase : ICommand
    {
        public abstract Task ExecuteAsync();
    }
}
