using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Configure
{
    [Command("project", HelpText = "Definir globalmente um projeto de trabalho.")]
    public class ConfigureProjectCommand : ConfigureCommandBase
    {
        [Parameter(HelpText = "Diretório do projeto.")]
        public string ProjectPath { get; set; }
        
        private readonly ISettingInfraService _settingsService;

        public ConfigureProjectCommand(ISettingInfraService settingsService) : base() 
        {
            _settingsService = settingsService;
        }

        public override async Task ExecuteAsync()
        {
            if (Directory.Exists(ProjectPath))
            {
                var projectFiles = Directory.GetFiles(ProjectPath, "*.csproj");
                if (!projectFiles.Any()) 
                    throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);
                
                if (projectFiles.Count() > 1) 
                    throw new ValidationException(
                        "Exist many projects in current directory. " +
                        "Please input project file name."
                    );
                
                ProjectPath = projectFiles.Single();
            } else if (File.Exists(ProjectPath))
            {
                if (Path.GetExtension(ProjectPath) != ".csproj")
                    throw new ValidationException("This file is not a project.");

                ProjectPath = ProjectPath.Replace("/", @"\");
            } else throw new ProjectNotFoundException(ProjectTypeEnumerator.WEB_API);

            await _settingsService.SetStringAsync(SettingEnumerator.PROJECT_DIRECTORY, ProjectPath);
        }
    }
}
