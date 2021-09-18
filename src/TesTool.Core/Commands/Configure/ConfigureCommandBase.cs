using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;

namespace TesTool.Core.Commands.Configure
{
    [Command("configure", "c")]
    public abstract class ConfigureCommandBase : ICommand
    {
        [Parameter]
        public string Directory { get; set; }

        public abstract Task ExecuteAsync();
    }
}
