using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("--convention", "-c", HelpText = "Definir arquivo de configuração de conveção.")]
    public class ConfigureConventionCommand : ConfigureCommandBase
    {
        [Parameter(IsDefault = true, HelpText = "Diretório do arquivo.")]
        public override string Path { get; set; }

        public ConfigureConventionCommand(
            ISettingsService settingsService,
            ILoggerService loggerService
        ) : base(
            loggerService, 
            settingsService, 
            SettingEnumerator.CONVENTION_PATH_FILE
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
