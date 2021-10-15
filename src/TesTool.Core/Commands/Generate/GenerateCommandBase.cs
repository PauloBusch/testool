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

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        private readonly IEnvironmentInfraService _environmentInfraService;

        protected GenerateCommandBase(IEnvironmentInfraService environmentInfraService)
        {
            _environmentInfraService = environmentInfraService;
        }

        public abstract Task ExecuteAsync();

        protected string GetOutputDirectory() => string.IsNullOrWhiteSpace(Output)
            ? _environmentInfraService.GetWorkingDirectory() : Output;
    }
}
