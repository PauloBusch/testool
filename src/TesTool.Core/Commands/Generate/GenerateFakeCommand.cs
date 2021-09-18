using System;
using System.Threading.Tasks;
using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Generate
{
    [Command("--fake", "-f")]
    public class GenerateFakeCommand : GenerateCommandBase
    {
        [Parameter(IsDefault = true)]
        public string ClassName { get; set; }

        [Parameter(IsDefault = true)]
        public string FactoryName { get; set; }

        public override Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
