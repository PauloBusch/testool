using System.Threading.Tasks;
using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Configure
{
    [Command("--project", "-p")]
    public class ConfigureProjectCommand : ConfigureCommandBase
    {
        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
