using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("project", "p", HelpText = "Definir globalmente um projeto de trabalho.")]
    public class ConfigureProjectCommand : ConfigureCommandBase
    {
        [Parameter(HelpText = "Diretório do projeto.")]
        public string ProjectPath { get; set; }
        
        private readonly ILoggerInfraService _loggerService;
        private readonly ISettingInfraService _settingsService;

        public ConfigureProjectCommand(
            ILoggerInfraService loggerService,
            ISettingInfraService settingsService
        ) : base() 
        {
            _loggerService = loggerService;
            _settingsService = settingsService;
        }

        public override async Task ExecuteAsync()
        {
            if (Directory.Exists(ProjectPath))
            {
                var projectFiles = Directory.GetFiles(ProjectPath, "*.csproj");
                if (!projectFiles.Any())
                {
                    _loggerService.LogError("Project file is not found.");
                    return;
                } else if (projectFiles.Count() > 1)
                {
                    _loggerService.LogError("Exist many projects in current directory.");
                    return;
                }
                ProjectPath = projectFiles.Single();
            } else if (File.Exists(ProjectPath))
            {
                if (Path.GetExtension(ProjectPath) != ".csproj")
                {
                    _loggerService.LogError("This file is not a project.");
                    return;
                }
                ProjectPath = ProjectPath.Replace("/", @"\");
            } else
            {
                _loggerService.LogError("Project file is not found.");
                return;
            }

            await _settingsService.SetStringAsync(SettingEnumerator.PROJECT_DIRECTORY.Key, ProjectPath);
        }
    }
}
