using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Fakers
{
    [Command("faker", Order = 5, HelpText = "Gerar código de fabricação de objeto.")]
    public abstract class GenerateFakeBase : GenerateCommandBase
    {
        [Parameter(HelpText = "Nome da classe a ser fábricada.")]
        public string ClassName { get; set; }

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        protected GenerateFakeBase(IEnvironmentInfraService environmentInfraService) 
            : base(environmentInfraService) { }
    }
}
