using System.Threading.Tasks;
using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Configure
{
    [Command("convention", "c")]
    public class ConfigureConventionCommand : ConfigureCommandBase
    {
        [Parameter]
        public string Directory { get; set; }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
