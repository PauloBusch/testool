using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;

namespace TesTool.Core.Commands.Generate
{
    [Command("generate", "g", HelpText = "Gerar código C#.")]
    public abstract class GenerateCommandBase : ICommand
    {
        [Option(HelpText = "Diretório de saída.")]
        public string Output { get; set; }

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        public abstract Task ExecuteAsync();
    }
}
