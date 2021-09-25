using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models;

namespace TesTool.Core.Commands.Configure
{
    [Command("--configure", "-c")]
    public abstract class ConfigureCommandBase : ICommand
    {
        public abstract string Path { get; set; }

        protected readonly Setting _setting;
        protected readonly ILoggerService _loggerService;
        protected readonly ISettingsService _settingsService;

        public ConfigureCommandBase(
            ILoggerService loggerService,
            ISettingsService settingsService,
            Setting setting
        )
        {
            _loggerService = loggerService;
            _settingsService = settingsService;
            _setting = setting;
        }

        public virtual async Task ExecuteAsync()
        {
            await _settingsService.SetStringAsync(_setting.Key, Path);
        }
    }
}
