﻿using System.Threading.Tasks;
using TesTool.Core.Attributes;

namespace TesTool.Core.Commands.Generate
{
    [Command("project", "p", HelpText = "Gerar código de teste a partir de projeto.")]
    public class GenerateProjectCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Diretório do projeto.")]
        public string Directory { get; set; }

        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}