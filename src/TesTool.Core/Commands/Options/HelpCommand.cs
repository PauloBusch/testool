using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;

namespace TesTool.Core.Commands.Help
{
    [Command("--help", "-h")]
    public class HelpCommand : ICommand
    {
        [Parameter(IsDefault = true)]
        public string Command { get; set; }

        public Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
