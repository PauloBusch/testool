using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate
{
    [Command("generate", "g", HelpText = "Gerar código C#.")]
    public abstract class GenerateCommandBase : ICommand
    {
        [Option(HelpText = "Diretório de saída.")]
        public string Output { get; set; }
                
        protected readonly IFileSystemInfraService _fileSystemInfraService;

        protected GenerateCommandBase(
            IFileSystemInfraService  fileSystemInfraService
        )
        {
            _fileSystemInfraService = fileSystemInfraService;
        }

        protected abstract Task GenerateAsync();

        public async Task ExecuteAsync(ICommandContext context)
        {
            if (!string.IsNullOrWhiteSpace(Output) && !await _fileSystemInfraService.FileExistAsync(Output))
                throw new DirectoryNotFoundException("Diretório de saída inválido.");

            await GenerateAsync();
        }
    }
}
