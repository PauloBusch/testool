using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("model", HelpText = "Gerar fábrica de modelo de transporte de dados (DTO).")]
    public class GenerateFactoryModelCommand : GenerateFactoryBase
    {
        public GenerateFactoryModelCommand(IEnvironmentInfraService environmentInfraService) 
            : base(environmentInfraService) { }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
