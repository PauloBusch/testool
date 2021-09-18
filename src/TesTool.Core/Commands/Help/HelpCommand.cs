using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Help
{
    [Command("help", "h")]
    public class HelpCommand
    {
        [Parameter(IsDefault = true)]
        public string Command { get; set; }
    }
}
