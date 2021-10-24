using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate
{
    [Command("project", Order = 1, HelpText = "Gerar código de teste a partir de projeto.")]
    public class GenerateProjectCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Diretório do projeto.")]
        public string Directory { get; set; }

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        public GenerateProjectCommand(
            IFileSystemInfraService fileSystemInfraService
        ) : base(fileSystemInfraService) { }

        protected override Task GenerateAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
