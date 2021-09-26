using System.Threading.Tasks;
using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Generate
{
    [Command("controller", "c", HelpText = "Gerar código de teste a partir de controlador.")]
    public class GenerateControllerCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Nome da classe controlador.")]
        public string Controller { get; set; }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
