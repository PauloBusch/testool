using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("entity", HelpText = "Gerar fábrica de entidades de banco de dados.")]
    public class GenerateFactoryEntityCommand : GenerateFactoryBase
    {
        public GenerateFactoryEntityCommand(IEnvironmentInfraService environmentInfraService) 
            : base(environmentInfraService) { }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
