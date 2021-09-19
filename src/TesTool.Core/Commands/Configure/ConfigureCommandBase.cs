using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("--configure", "-c")]
    [Help("")]
    public abstract class ConfigureCommandBase : ICommand
    {
        [Parameter(IsDefault = true)]
        public string Directory { get; set; }

        public static string SETTINGS_KEY;
        private readonly ISettingsService _settingsService;

        public ConfigureCommandBase(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task ExecuteAsync()
        {
            await _settingsService.SetStringAsync(SETTINGS_KEY, Directory);
        }
    }
}
