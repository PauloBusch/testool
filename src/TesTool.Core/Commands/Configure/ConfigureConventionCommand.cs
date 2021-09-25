using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("convention", "c", HelpText = "Definir arquivo de configuração de conveção.")]
    public class ConfigureConventionCommand : ConfigureCommandBase
    {
        [Parameter(IsDefault = true, HelpText = "Diretório do arquivo de configuração.")]
        public string ConfigurationPath { get; set; }

        private readonly ILoggerInfraService _loggerService;
        private readonly ISettingInfraService _settingsService;

        public ConfigureConventionCommand(
            ISettingInfraService settingsService,
            ILoggerInfraService loggerService
        ) : base()
        {
            _loggerService = loggerService;
            _settingsService = settingsService;
        }

        public override async Task ExecuteAsync()
        {
            if (!File.Exists(ConfigurationPath))
            {
                _loggerService.LogError("File convention is not found.");
                return;
            }

            await _settingsService.SetStringAsync(SettingEnumerator.CONVENTION_PATH_FILE.Key, ConfigurationPath);
        }
    }
}
