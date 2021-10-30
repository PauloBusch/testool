using System.Collections.Generic;
using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface ICmdInfraService
    {
        Task ExecuteCommandsAsync(IEnumerable<string> commands);
    }
}
