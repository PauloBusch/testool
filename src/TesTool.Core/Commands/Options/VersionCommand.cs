using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Options
{
    [Flag]
    [Command("--version", "-v", HelpText = "Exiba a versão do TesTool em uso.")]
    public class VersionCommand : ICommand
    {
        private readonly ITesToolInfraService _tesToolInfraService;
        private readonly ILoggerInfraService _loggerService;

        public VersionCommand(
            ITesToolInfraService tesToolInfraService,
            ILoggerInfraService loggerService
        )
        {
            _tesToolInfraService = tesToolInfraService;
            _loggerService = loggerService;
        }

        public Task ExecuteAsync(ICommandContext context)
        {
            _loggerService.LogInformation(_tesToolInfraService.GetVersion());
            return Task.CompletedTask;
        }
    }
}
