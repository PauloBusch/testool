using System;
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
    [Flag] [Default]
    [Command("--help", "-h", HelpText = "Mostrar ajuda de linha de comando.")]
    public class HelpCommand : ICommand
    {
        [Parameter(
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

        public async Task ExecuteAsync()
        {
            if (string.IsNullOrWhiteSpace(Command))
            {
                LogDefaultCommands();
                return;
            }

            var arguments = Command.Split(" ");
            var commandType = _commandExplorerService.GetCommandTypeExact(arguments);
            if (commandType != null)
            {
                LogExactCommand(commandType);
                return;
            }

            var commandTypesMatched = _commandExplorerService.GetCommandTypesMatched(arguments);
            if (commandTypesMatched.Any())
            {
                LogSearchCommands(commandTypesMatched);
                return;
            }

            _loggerService.LogError("Comando não encontrado.");
            _loggerService.LogInformation("Execute 'testool --help' para ver os comandos válidos.");
            await Task.CompletedTask;
        }

        private void LogDefaultCommands()
        {
            var template = "  {0,-20} {1}";
            var commandTypes = _commandExplorerService.GetAllCommandTypes();
            var optionHelperTexts = commandTypes
                .Where(type => type.GetCustomAttributes<FlagAttribute>().Any())
                .Select(type => type.GetCustomAttributes<CommandAttribute>().Reverse())
                .Select(commands => commands.First())
                .Distinct()
                .Select(command =>
                {
                    var commandOptions = !string.IsNullOrWhiteSpace(command.Alias) ? $"{command.Alias}, {command.Name}" : command.Name;
                    return string.Format(template, commandOptions, command.HelpText);
                })
                .ToArray();
            var commandHelperTexts = commandTypes
                .Where(type => !type.GetCustomAttributes<FlagAttribute>().Any())
                .Select(type => type.GetCustomAttributes<CommandAttribute>().Reverse())
                .Select(commands => commands.First())
                .Distinct()
                .Select(command =>
                {
                    var commandOptions = !string.IsNullOrWhiteSpace(command.Alias) ? $"{command.Alias}|{command.Name}" : command.Name;
                    return string.Format(template, commandOptions, command.HelpText);
                })
                .ToArray();

            LogConsole(default, new string[0], options: optionHelperTexts, commands: commandHelperTexts);
        }

        private void LogSearchCommands(IEnumerable<Type> commandTypes)
        {
            var template = "  {0,-30} {1}";
            var commandBaseType = commandTypes.FirstOrDefault()?.BaseType;
            var commandHelperTexts = commandTypes
                .Where(type => !type.GetCustomAttributes<OptionAttribute>().Any())
                .Select(type => type.GetCustomAttributes<CommandAttribute>().Reverse())
                .Select(command => command.Last())
                .Distinct()
                .Select(command =>
                {
                    var commandOptions = !string.IsNullOrWhiteSpace(command.Alias) ? $"{command.Alias}|{command.Name}" : command.Name;
                    return string.Format(template, commandOptions, command.HelpText);
                })
                .ToArray();

            LogConsole(commandBaseType, new string[0], new string[0], commands: commandHelperTexts);
        }

        private void LogExactCommand(Type commandType)
        {
            var template = "  {0,-30} {1}";
            var commandProperties = commandType.GetProperties();
            var commandTypes = _commandExplorerService.GetAllCommandTypes();
            var argumentHelperTexts = commandProperties
                .Select(p => new { Name = p.Name, Parameter = p.GetCustomAttribute<ParameterAttribute>() })
                .Where(p => p.Parameter is not null)
                .Select(p => string.Format(template, $"<{p.Name.ToSnakeCase().ToUpper()}>", p.Parameter.HelpText))
                .ToArray();
            var optionsHelperTexts = commandProperties
                .Where(t => t.GetCustomAttributes<OptionAttribute>().Any())
                .Select(p => new { Name = p.Name, Option = p.GetCustomAttribute<OptionAttribute>() })
                .Select(p => string.Format(template, $"-{p.Name.ToLower().First()}, --{p.Name.ToLower()} <{p.Name.ToSnakeCase().ToUpper()}>", p.Option.HelpText))
                .ToArray();
            var globalFalgsHelperTexts = commandTypes
                .Where(t => t.GetCustomAttributes<DefaultAttribute>().Any())
                .Where(t => t.GetCustomAttributes<FlagAttribute>().Any())
                .Select(p => new { Name = p.Name, Command = p.GetCustomAttribute<CommandAttribute>(), Flag = p.GetCustomAttribute<FlagAttribute>() })
                .Select(p => string.Format(template, $"{p.Command.Alias.ToLower()}, {p.Command.Name.ToLower()}", p.Command.HelpText))
                .ToArray();
            var flagsHelperTexts = commandProperties
                .Where(t => t.GetCustomAttributes<FlagAttribute>().Any())
                .Select(p => new { Name = p.Name, Flag = p.GetCustomAttribute<FlagAttribute>() })
                .Select(p => string.Format(template, $"-{p.Name.ToLower().First()}, --{p.Name.ToLower()}", p.Flag.HelpText))
                .ToArray();

            LogConsole(commandType, argumentHelperTexts, globalFalgsHelperTexts.Concat(optionsHelperTexts.Concat(flagsHelperTexts)), new string[0]);
        }

        private void LogConsole(
            Type commandType,
            IEnumerable<string> arguments,
            IEnumerable<string> options,
            IEnumerable<string> commands
        )
        {
            var cliCommands = new List<string>();
            if (commandType is not null)
            {
                var calls = commandType
                    .GetCustomAttributes<CommandAttribute>()
                    .Reverse()
                    .Select(c => c.Name);
                cliCommands.AddRange(calls);
            }
            var cliLabels = new List<string>();
            if (commands.Count() > 1) cliLabels.Add("command");
            if (arguments.Any()) cliLabels.Add("arguments");
            if (options.Any()) cliLabels.Add("options");
            _loggerService.LogInformation($"\nUso: testool {string.Join(" ", cliCommands.Concat(cliLabels.Select(c => $"[{c}]")))}");

            if (arguments.Any())
            {
                _loggerService.LogInformation("\nArgumentos:");
                _loggerService.LogInformation(string.Join("\n", arguments));
            }

            if (options.Any())
            {
                _loggerService.LogInformation("\nOpções:");
                _loggerService.LogInformation(string.Join("\n", options.OrderBy(o => o)));
            }

            if (commands.Any())
            {
                _loggerService.LogInformation("\nComandos:");
                _loggerService.LogInformation(string.Join("\n", commands.OrderBy(c => c)));
                _loggerService.LogInformation($"\nExecute 'testool --help {string.Join(" ", cliCommands.Concat(new[] { "[command]" }))}' para obter mais informações sobre um comando.");
            }
        }
    }
}
