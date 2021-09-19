using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;

namespace TesTool.Core.Commands.Options
{
    [Command("--version", "-v")]
    public class VersionCommand : ICommand
    {
        private readonly ILoggerService<VersionCommand> _logger;

        public VersionCommand(ILoggerService<VersionCommand> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            _logger.LogInformation(version);
            return Task.CompletedTask;
        }
    }
}
