using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("--convention", "-c")]
    public class ConfigureConventionCommand : ConfigureCommandBase
    {
        static ConfigureConventionCommand() { SETTINGS_KEY = "CONVENTION_PATH_FILE"; }

        public ConfigureConventionCommand(ISettingsService settingsService) : base(settingsService) { }
    }
}
