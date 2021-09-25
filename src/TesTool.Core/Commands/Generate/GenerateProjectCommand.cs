using System.Threading.Tasks;
using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Generate
{
    [Command("--project", "-p", HelpText = "Gera código de teste a partir de projeto.")]
    public class GenerateProjectCommand : GenerateCommandBase
    {
        [Parameter(IsDefault = true, HelpText = "Diretório do projeto.")]
        public string Directory { get; set; }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
