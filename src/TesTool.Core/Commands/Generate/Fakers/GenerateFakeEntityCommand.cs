using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Fakers
{
    [Command("entity", HelpText = "Gerar código de fabricação de entidade de banco de dados.")]
    public class GenerateFakeEntityCommand : GenerateFakeBase
    {
        public GenerateFakeEntityCommand(IEnvironmentInfraService environmentInfraService) 
            : base(environmentInfraService) { }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
