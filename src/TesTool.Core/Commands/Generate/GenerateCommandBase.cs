using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate
{
    [Command("generate", "g", HelpText = "Gerar código C#.")]
    public abstract class GenerateCommandBase : ICommand
    {
        [Option(HelpText = "Diretório de saída.")]
        public string Output { get; set; }

        public abstract Task ExecuteAsync(ICommandContext context);
    }
}
