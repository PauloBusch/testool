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

        protected readonly ILoggerInfraService _loggerInfraService;

        protected GenerateCommandBase(ILoggerInfraService loggerInfraService)
        {
            _loggerInfraService = loggerInfraService;
        }

        public abstract Task GenerateAsync(ICommandContext context);
        public virtual async Task ExecuteAsync(ICommandContext context)
        {
            if (!context.ExecutionCascade)
                _loggerInfraService.LogInformation($"Escaneando projeto.");
            
            await GenerateAsync(context);
            
            if (!context.ExecutionCascade) 
                _loggerInfraService.LogInformation("Código gerado com sucesso. Realize uma revisão antes de comitar.");
        }
    }
}
