using System.Threading.Tasks;
using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Generate
{
    [Command("compare", "c")]
    public class GenerateCompareCommand : GenerateCommandBase
    {
        [Parameter(IsDefault = true)]
        public string SourceClassName { get; set; }

        [Parameter(IsDefault = true)]
        public string TargetClassName { get; set; }

        [Parameter(IsDefault = true)]
        public string ComparatorName { get; set; }

        [Flag]
        public string Static { get; set; }
        
        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
