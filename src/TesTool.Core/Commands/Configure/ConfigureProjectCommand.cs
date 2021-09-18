using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("--project", "-p")]
    public class ConfigureProjectCommand : ConfigureCommandBase
    {
        static ConfigureProjectCommand() { SETTINGS_KEY = "PROJECT_DIRECTORY"; }

        public ConfigureProjectCommand(ISettingsService settingsService) : base(settingsService) { }
    }
}
