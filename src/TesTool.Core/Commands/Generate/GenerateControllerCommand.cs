using System.Threading.Tasks;
using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Generate
{
    [Command("--controller", "-c")]
    public class GenerateControllerCommand : GenerateCommandBase
    {
        [Parameter(IsDefault = true)]
        public string Controller { get; set; }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
