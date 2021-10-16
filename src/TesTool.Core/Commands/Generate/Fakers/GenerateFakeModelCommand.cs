using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Fakers
{
    [Command("model", HelpText = "Gerar código de fabricação de modelo de transporte de dados (DTO).")]
    public class GenerateFakeModelCommand : GenerateFakeBase
    {
        public GenerateFakeModelCommand(IEnvironmentInfraService environmentInfraService) 
            : base(environmentInfraService) { }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
