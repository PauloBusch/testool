using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("factory", Order = 3, HelpText = "Gerar código de chamadas de fabricação.")]
    public abstract class GenerateFactoryBase : GenerateCommandBase
    {
        [Parameter(IsRequired = false, HelpText = "Nome da classe fábrica.")]
        public string FactoryName { get; set; }

        protected GenerateFactoryBase(IEnvironmentInfraService environmentInfraService) 
            : base(environmentInfraService) { }
    }
}
