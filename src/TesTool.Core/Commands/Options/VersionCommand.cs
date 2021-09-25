using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;

namespace TesTool.Core.Commands.Options
{
    [Command("--version", "-v", HelpText = "Exibir a versão do TesTool em uso.")]
    public class VersionCommand : ICommand
    {
        private readonly ILoggerService _loggerService;

        public VersionCommand(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public Task ExecuteAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            _loggerService.LogInformation(version);
            return Task.CompletedTask;
        }
    }
}
