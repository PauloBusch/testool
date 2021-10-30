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
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = _environmentInfraService.GetWorkingDirectory();
            // TODO: uncomment
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {string.Join(" && ", commands)}";
            process.StartInfo = startInfo;
            process.Start();
            await process.WaitForExitAsync();
        }
    }
}
