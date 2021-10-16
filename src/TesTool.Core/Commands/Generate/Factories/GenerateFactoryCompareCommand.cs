using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("compare", HelpText = "Gerar fábrica de objetos de comparação.")]
    public class GenerateFactoryCompareCommand : GenerateFactoryBase
    {
        protected GenerateFactoryCompareCommand(IEnvironmentInfraService environmentInfraService) 
            : base(environmentInfraService) { }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
