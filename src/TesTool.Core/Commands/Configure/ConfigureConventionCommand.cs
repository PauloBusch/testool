using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("convention", "c", HelpText = "Definir arquivo de configuração de conveção.")]
    public class ConfigureConventionCommand : ConfigureCommandBase
    {
        [Parameter(HelpText = "Diretório do arquivo de configuração.")]
        public string ConfigurationPath { get; set; }

        private readonly ISettingInfraService _settingsService;

        public ConfigureConventionCommand(ISettingInfraService settingsService) : base()
        {
            _settingsService = settingsService;
        }

        public override async Task ExecuteAsync()
        {
            if (!File.Exists(ConfigurationPath)) 
                throw new FileNotFoundException(Path.GetFileName(ConfigurationPath));

            if (Path.GetExtension(ConfigurationPath) != ".json")
                throw new ValidationException("Only accept json file format.");

            await _settingsService.SetStringAsync(SettingEnumerator.CONVENTION_PATH_FILE.Key, ConfigurationPath);
        }
    }
}
