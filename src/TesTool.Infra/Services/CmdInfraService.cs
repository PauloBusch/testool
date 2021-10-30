using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class CmdInfraService : ICmdInfraService
    {
        private readonly IEnvironmentInfraService _environmentInfraService;

        public CmdInfraService(IEnvironmentInfraService environmentInfraService)
        {
            _environmentInfraService = environmentInfraService;
        }

        public async Task ExecuteCommandsAsync(IEnumerable<string> commands)
        {
            var startInfo = new ProcessStartInfo { 
                WorkingDirectory = _environmentInfraService.GetWorkingDirectory(),
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"/C {string.Join(" && ", commands)}"
            };
            var process = new Process { 
                EnableRaisingEvents = true,
                StartInfo = startInfo
            };
            process.Start();
            await process.WaitForExitAsync();
        }
    }
}
