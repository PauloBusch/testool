using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("--project", "-p", HelpText = "Definir globalmente um projeto de trabalho.")]
    public class ConfigureProjectCommand : ConfigureCommandBase
    {
        [Parameter(IsDefault = true, HelpText = "Diretório do projeto.")]
        public override string Path { get; set; }

        public ConfigureProjectCommand(
            ILoggerService loggerService,
            ISettingsService settingsService
        ) : base(
            loggerService, 
            settingsService,
            SettingEnumerator.PROJECT_DIRECTORY
        ) { }

        public override async Task ExecuteAsync()
        {
            if (Directory.Exists(Path))
            {
                var projectFiles = Directory.GetFiles(Path, "*.csproj");
                if (!projectFiles.Any())
                {
                    _loggerService.LogError("Project file is not found.");
                    return;
                } else if (projectFiles.Count() > 1)
                {
                    _loggerService.LogError("Exist many projects in current directory.");
                    return;
                }
                Path = projectFiles.Single();
            } else if (File.Exists(Path))
            {
                if (System.IO.Path.GetExtension(Path) != ".csproj")
                {
                    _loggerService.LogError("This file is not a project.");
                    return;
                }
                Path = Path.Replace("/", @"\");
            } else
            {
                _loggerService.LogError("Project file is not found.");
                return;
            }

            await base.ExecuteAsync();
        }
    }
}
