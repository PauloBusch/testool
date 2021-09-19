using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enums;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("--convention", "-c")]
    public class ConfigureConventionCommand : ConfigureCommandBase
    {
        public ConfigureConventionCommand(
            ISettingsService settingsService,
            ILoggerService loggerService
        ) : base(
            loggerService, 
            settingsService, 
            Setting.CONVENTION_PATH_FILE
        ) { }

        public override async Task ExecuteAsync()
        {
            if (!File.Exists(Path))
            {
                _loggerService.LogError("File convention is not found.");
                return;
            }

            await base.ExecuteAsync();
        }
    }
}
