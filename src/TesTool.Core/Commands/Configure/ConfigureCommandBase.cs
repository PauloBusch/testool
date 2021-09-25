using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;

namespace TesTool.Core.Commands.Configure
{
    [Command("configure", "c", HelpText = "Configurar variáveis globais.")]
    public abstract class ConfigureCommandBase : ICommand
    {
        public abstract Task ExecuteAsync();
    }
}
