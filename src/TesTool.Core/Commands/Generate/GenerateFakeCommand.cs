using System;
using System.Threading.Tasks;
using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Generate
{
    [Command("fake", "f", HelpText = "Gerar código de fabricação de objeto.")]
    public class GenerateFakeCommand : GenerateCommandBase
    {
        [Parameter(IsDefault = true, HelpText = "Nome da classe a ser fábricada.")]
        public string ClassName { get; set; }

        [Parameter(IsDefault = true, HelpText = "Nome da classe que terá o método de fabricação.")]
        public string FactoryName { get; set; }

        public override Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
