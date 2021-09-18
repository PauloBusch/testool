using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;

namespace TesTool.Core.Commands.Generate
{
    [Command("--generate", "-g")]
    public abstract class GenerateCommandBase : ICommand
    {
        [Parameter(IsRequired = false)]
        public string Output { get; set; }

        [Flag]
        public bool Static { get; set; }

        public abstract Task ExecuteAsync();
    }
}
