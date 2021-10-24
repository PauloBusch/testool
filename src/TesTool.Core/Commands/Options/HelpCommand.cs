using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Exceptions;
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

        public async Task ExecuteAsync(ICommandContext context)
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
            if (commandTypesMatched.Any()) LogSearchCommands(commandTypesMatched, arguments.Length);
            else throw new CommandNotFoundException("Execute 'testool --help' para ver os comandos válidos.");

            await Task.CompletedTask;
        }

        private void LogDefaultCommands()
        {
            var commandTypes = _commandExplorerService.GetAllCommandTypes();
            var optionHelperTexts = commandTypes
                .Where(type => type.GetCustomAttributes<FlagAttribute>().Any())
                .Select(type => type.GetCustomAttributes<CommandAttribute>().Reverse())
                .Select(commands => commands.First())
                .Distinct()
                .OrderBy(c => c.Order)
                .ThenBy(c => c.Name)
                .Select(command =>
                {
                    var commandOptions = !string.IsNullOrWhiteSpace(command.Alias) ? $"{command.Alias}, {command.Name}" : command.Name;
                    return new [] { commandOptions, command.HelpText };
                })
                .ToArray();
            var commandHelperTexts = commandTypes
                .Where(type => !type.GetCustomAttributes<FlagAttribute>().Any())
                .Select(type => type.GetCustomAttributes<CommandAttribute>().Reverse())
                .Select(commands => commands.First())
                .Distinct()
                .OrderBy(c => c.Order)
                .ThenBy(c => c.Name)
                .Select(command =>
                {
                    var commandOptions = !string.IsNullOrWhiteSpace(command.Alias) ? $"{command.Alias}|{command.Name}" : command.Name;
                    return new[] { commandOptions, command.HelpText };
                })
                .ToArray();

            LogConsole(default, Array.Empty<string[]>(), optionsColumns: optionHelperTexts, commandsColumns: commandHelperTexts);
        }

        private void LogSearchCommands(IEnumerable<Type> commandTypes, int commandLevel)
        {
            var commandBaseType = commandTypes.FirstOrDefault()?.BaseType;
            var commandHelperTexts = commandTypes
                .Where(type => !type.GetCustomAttributes<OptionAttribute>().Any())
                .Select(type => type.GetCustomAttributes<CommandAttribute>().Reverse().ElementAt(commandLevel))
                .Distinct()
                .OrderBy(c => c.Order)
                .ThenBy(c => c.Name)
                .Select(command =>
                {
                    var commandOptions = !string.IsNullOrWhiteSpace(command.Alias) ? $"{command.Alias}|{command.Name}" : command.Name;
                    return new[] { commandOptions, command.HelpText };
                })
                .ToArray();

            LogConsole(commandBaseType, Array.Empty<string[]>(), Array.Empty<string[]>(), commandsColumns: commandHelperTexts);
        }

        private void LogExactCommand(Type commandType)
        {
            var commandProperties = commandType.GetProperties();
            var commandTypes = _commandExplorerService.GetAllCommandTypes();
            var argumentHelperTexts = commandProperties
                .Select(p => new { Name = p.Name, Parameter = p.GetCustomAttribute<ParameterAttribute>() })
                .Where(p => p.Parameter is not null)
                .Select(p => new []{ $"<{p.Name.ToSnakeCase().ToUpper()}>", p.Parameter.HelpText })
                .ToArray();
            var optionsHelperTexts = commandProperties
                .Where(t => t.GetCustomAttributes<OptionAttribute>().Any())
                .Select(p => new { Name = p.Name, Option = p.GetCustomAttribute<OptionAttribute>() })
                .OrderBy(p => p.Name)
                .Select(p => new [] { $"-{p.Name.ToLower().First()}, --{p.Name.ToLower()} <{p.Name.ToSnakeCase().ToUpper()}>", p.Option.HelpText })
                .ToArray();
            var globalFalgsHelperTexts = commandTypes
                .Where(t => t.GetCustomAttributes<DefaultAttribute>().Any())
                .Where(t => t.GetCustomAttributes<FlagAttribute>().Any())
                .Select(p => p.GetCustomAttribute<CommandAttribute>())
                .OrderBy(p => p.Alias ?? p.Name)
                .Select(p => new [] { $"{p.Alias.ToLower()}, {p.Name.ToLower()}", p.HelpText })
                .ToArray();
            var flagsHelperTexts = commandProperties
                .Where(t => t.GetCustomAttributes<FlagAttribute>().Any())
                .Select(p => new { Name = p.Name, Flag = p.GetCustomAttribute<FlagAttribute>() })
                .OrderBy(p => p.Name)
                .Select(p => new [] { $"-{p.Name.ToLower().First()}, --{p.Name.ToLower()}", p.Flag.HelpText })
                .ToArray();

            LogConsole(commandType, argumentHelperTexts, globalFalgsHelperTexts.Concat(optionsHelperTexts.Concat(flagsHelperTexts)), new string[0][]);
        }

        private void LogConsole(
            Type commandType,
            IEnumerable<IEnumerable<string>> argumentsColumns,
            IEnumerable<IEnumerable<string>> optionsColumns,
            IEnumerable<IEnumerable<string>> commandsColumns
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
            if (commandsColumns.Count() > 1) cliLabels.Add("command");
            if (argumentsColumns.Any()) cliLabels.Add("arguments");
            if (optionsColumns.Any()) cliLabels.Add("options");
            _loggerService.LogInformation($"\nUso: testool {string.Join(" ", cliCommands.Concat(cliLabels.Select(c => $"[{c}]")))}");

            if (argumentsColumns.Any())
            {
                _loggerService.LogInformation("\nArgumentos:");
                _loggerService.LogInformation(FormatRow(argumentsColumns));
            }

            if (optionsColumns.Any())
            {
                _loggerService.LogInformation("\nOpções:");
                _loggerService.LogInformation(FormatRow(optionsColumns));
            }

            if (commandsColumns.Any())
            {
                _loggerService.LogInformation("\nComandos:");
                _loggerService.LogInformation(FormatRow(commandsColumns));
                _loggerService.LogInformation($"\nExecute 'testool --help {string.Join(" ", cliCommands.Concat(new[] { "[command]" }))}' para obter mais informações sobre um comando.");
            }
        }

        private static string FormatRow(IEnumerable<IEnumerable<string>> rows)
        {
            var steps = 5;
            var columns = rows.Select(r => r.First());
            var characters = columns.Max(c => c.Length);
            var columnSize = Math.Floor((decimal)characters / steps) * steps + 5;
            var template = $"  {{0,-{columnSize}}} {{1}}";
            return string.Join("\n", rows.Select(c => string.Format(template, c.ToArray())));
        }
    }
}
