using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Extensions;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Help
{
    [Command("--help", "-h", IsDefault = true, IsOption = true, HelpText = "Mostrar ajuda de linha de comando.")]
    public class HelpCommand : ICommand
    {
        [Parameter(
            IsDefault = true, 
            IsRequired = false,
            IsCumulative = true, 
            HelpText = "Ajuda para comando específico."
        )]
        public string Command { get; set; }

        private readonly ICommandExplorerService _commandExplorerService;
        private readonly ILoggerInfraService _loggerService;

        public HelpCommand(
            ICommandExplorerService commandExplorerService,
            ILoggerInfraService loggerService
        )
        {
            _commandExplorerService = commandExplorerService;
            _loggerService = loggerService;
        }

        public Task ExecuteAsync()
        {
            if (string.IsNullOrWhiteSpace(Command)) LogAllCommands();
            else LogCommandDetails();
            return Task.CompletedTask;
        }
        
        private void LogAllCommands()
        {
            var template = "  {0,-20} {1}";
            var commandTypes = _commandExplorerService.GetAllCommandTypes();
            var helperTexts = commandTypes
                .Select(type => type.GetCustomAttributes<CommandAttribute>().Reverse())
                .Select(commands => commands.First())
                .Distinct()
                .Select(command => {
                    var commandOptions = !string.IsNullOrWhiteSpace(command.Alias) ? $"{command.Alias}|{command.Name}" : command.Name;
                    return new { command.IsOption, HelpText = string.Format(template, commandOptions, command.HelpText) };
                })
                .ToArray();
            _loggerService.LogInformation("\nUso: testool [command] [options]");

            var options = helperTexts.Where(h => h.IsOption).ToArray();
            if (options.Any())
            {
                _loggerService.LogInformation("\nOpções:");
                _loggerService.LogInformation(string.Join("\n", options.Select(h => h.HelpText)));
            }
            
            var commands = helperTexts.Where(h => !h.IsOption).ToArray();
            if (commands.Any())
            {
                _loggerService.LogInformation("\nComandos:");
                _loggerService.LogInformation(string.Join("\n", commands.Select(h => h.HelpText)));
                _loggerService.LogInformation("\nExecute 'testool --help [command]' para obter mais informações sobre um comando.");
            }
        }

        private void LogCommandDetails()
        {
            var commandTypes = _commandExplorerService.GetCommandTypesMatched(Command?.Split(" "));
            if (!commandTypes.Any())
            {
                _loggerService.LogError("Comando não encontrado.");
                _loggerService.LogInformation("Execute 'testool --help' para ver os comandos válidos.");
                return;
            }

            var template = "  {0,-30} {1}";
            var commandAttributes = commandTypes
                .Select(type => type.GetCustomAttributes<CommandAttribute>().Reverse())
                .ToArray();
            var mainCommandType = commandTypes.First();
            var mainCommandCall = string.Join(" ", mainCommandType
                .GetCustomAttributes<CommandAttribute>()
                .Reverse()
                .Select(c => c.Name));
            var mainCommandProperties = mainCommandType.GetProperties();
            var argumentHelperTexts = mainCommandProperties
                .Select(p => new { Name = p.Name, Parameter = p.GetCustomAttribute<ParameterAttribute>() })
                .Where(p => p.Parameter is not null)
                .Select(p =>  string.Format(template, $"<{p.Name.ToSnakeCase().ToUpper()}>", p.Parameter.HelpText))
                .ToArray();
            var optionsHelperTexts = mainCommandProperties
                .Select(p => new { Name = p.Name, Flag = p.GetCustomAttribute<FlagAttribute>() })
                .Where(p => p.Flag is not null)
                .Select(p => string.Format(template, $"-{p.Name.ToLower().First()}|--{p.Name.ToLower()}", p.Flag.HelpText))
                .ToArray();
            var commandHelperTexts = commandAttributes
                .Select(command => command.Last())
                .Distinct()
                .Where(command => !command.IsOption)
                .Select(command => {
                    var commandOptions = !string.IsNullOrWhiteSpace(command.Alias) ? $"{command.Alias}|{command.Name}" : command.Name;
                    return string.Format(template, commandOptions, command.HelpText);
                })
                .ToArray();

            var cliParameters = new List<string>();
            if (commandTypes.Count() > 1) cliParameters.Add("command");
            if (argumentHelperTexts.Any()) cliParameters.Add("arguments");
            if (optionsHelperTexts.Any()) cliParameters.Add("options");
            _loggerService.LogInformation($"\nUso: testool {mainCommandCall} {string.Join(" ", cliParameters.Select(c => $"[{c}]"))}");

            if (argumentHelperTexts.Any())
            {
                _loggerService.LogInformation("\nArgumentos:");
                _loggerService.LogInformation(string.Join("\n", argumentHelperTexts));
            }

            if (optionsHelperTexts.Any())
            {
                _loggerService.LogInformation("\nOpções:");
                _loggerService.LogInformation(string.Join("\n", optionsHelperTexts));
            }

            if (commandHelperTexts.Any())
            {
                _loggerService.LogInformation("\nComandos:");
                _loggerService.LogInformation(string.Join("\n", commandHelperTexts));
            }
            _loggerService.LogInformation($"\nExecute 'testool --help {mainCommandCall} [command]' para obter mais informações sobre um comando.");
        }
    }
}
