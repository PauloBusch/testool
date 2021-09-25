using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;

namespace TesTool.Core.Commands.Help
{
    [Command("--help", "-h", IsDefault = true, HelpText = "Mostrar a ajuda de linha de comando.")]
    public class HelpCommand : ICommand
    {
        [Parameter(IsDefault = true, HelpText = "Ajuda para comando específico.")]
        public string Command { get; set; }

        public Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
