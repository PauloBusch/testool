using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("convention", HelpText = "Definir arquivo de configuração de conveção.")]
    public class ConfigureConventionCommand : ConfigureCommandBase
    {
        [Parameter(HelpText = "Diretório do arquivo de configuração.")]
        public string ConfigurationPath { get; set; }

        [Flag(HelpText = "Gera arquivo de configuração padrão.")]
        public bool Init { get; set; }

        private readonly ISettingInfraService _settingsService;
        private readonly IConventionInfraService _conventionInfraService;
        private readonly IFileSystemInfraService _fileSystemInfraService;
        private readonly ISerializerInfraService _serializerInfraService;

        public ConfigureConventionCommand(
            ISettingInfraService settingsService,
            IConventionInfraService conventionInfraService,
            IFileSystemInfraService fileSystemInfraService,
            ISerializerInfraService serializerInfraService
        ) : base()
        {
            _settingsService = settingsService;
            _conventionInfraService = conventionInfraService;
            _fileSystemInfraService = fileSystemInfraService;
            _serializerInfraService = serializerInfraService;
        }

        public override async Task ExecuteAsync()
        {
            if (Init && await _fileSystemInfraService.FileExistAsync(ConfigurationPath))
                throw new DuplicatedSourceFileException(Path.GetFileName(ConfigurationPath));

            if (!Init && !await _fileSystemInfraService.FileExistAsync(ConfigurationPath)) 
                throw new FileNotFoundException(Path.GetFileName(ConfigurationPath));

            if (Path.GetExtension(ConfigurationPath) != ".json")
                throw new ValidationException("Only accept json file format.");

            if (Init)
            {
                var conventions = await _conventionInfraService.GetConfiguredConventionsAsync();
                var jsonConventions = _serializerInfraService.SerializeIndented(conventions);
                await _fileSystemInfraService.SaveFileAsync(ConfigurationPath, jsonConventions);
            }

            await _settingsService.SetStringAsync(SettingEnumerator.CONVENTION_PATH_FILE, ConfigurationPath);
        }
    }
}
