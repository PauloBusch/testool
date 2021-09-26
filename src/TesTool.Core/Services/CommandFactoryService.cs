using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TesTool.Core.Attributes;
using TesTool.Core.Extensions;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Services
{
    public class CommandFactoryService : ICommandFactoryService
    {
        private readonly ICommandExplorerService _commandExplorerService;
        private readonly ILoggerInfraService _loggerService;
        private readonly IServiceProvider _serviceProvider;

        public CommandFactoryService(
            ICommandExplorerService commandExplorerService,
            ILoggerInfraService loggerService,
            IServiceProvider serviceProvider
        )
        {
            _commandExplorerService = commandExplorerService;
            _serviceProvider = serviceProvider;
            _loggerService = loggerService;
        }

        public ICommand CreateCommand(string[] arguments)
        {
            if (arguments == null || !arguments.Any() || arguments.All(a => string.IsNullOrWhiteSpace(a)))
            {
                var defaultCommandType = _commandExplorerService.GetAllCommandTypes()
                    .SingleOrDefault(type => type.GetCustomAttributes<DefaultAttribute>().Any());
                if (defaultCommandType is not null) return _serviceProvider.GetService(defaultCommandType) as ICommand;
                _loggerService.LogError("Nenhum parâmetro foi informado.");
                return default;
            }

            var commandTypesMatched = _commandExplorerService.GetCommandTypesMatched(arguments);
            if (!commandTypesMatched.Any())
            {
                _loggerService.LogError("Comando não encontrado.");
                _loggerService.LogInformation("Execute 'testool --help' para ver os comandos válidos.");
                return default;
            }

            var commandType = _commandExplorerService.GetCommandTypeExact(arguments);
            if (commandType == null)
            {
                _loggerService.LogError("Comando incompleto.");
                _loggerService.LogInformation("Execute 'testool --help [command]' para ver mais informações sobre o comando.");
                return default;
            }

            var command = _serviceProvider.GetService(commandType) as ICommand;
            var properties = commandType.GetProperties();
            var argumentsStack = new List<string>(arguments);
            if (commandType.GetCustomAttributes<DefaultAttribute>().Any())
            {
                var commands = commandType.GetCustomAttributes<CommandAttribute>();
                argumentsStack.RemoveAll(a => commands.Any(c => c.Equals(a)));
            } else argumentsStack.RemoveRange(0, commandType.GetCustomAttributes<CommandAttribute>().Count());
            foreach (var property in properties)
            {
                var flagAttribute = property.GetCustomAttribute<FlagAttribute>();
                var propertyAttribute = property.GetCustomAttribute<ParameterAttribute>();
                var optionAttribute = property.GetCustomAttribute<OptionAttribute>();

                if (optionAttribute is not null || flagAttribute is not null)
                {
                    var matchArguments = new[] {
                        $"--{property.Name.ToLower()}",
                        $"-{property.Name.ToLower().First()}"
                    };

                    if (!argumentsStack.Any(a => matchArguments.Contains(a))) continue;

                    if (flagAttribute is not null) property.SetValue(command, true, null);
                    if (optionAttribute is not null)
                    {
                        var existIndex = argumentsStack.Count >= 2;
                        var validArgument = existIndex && !argumentsStack.ElementAt(1).StartsWith("-");
                        if (validArgument)
                        {
                            var value = argumentsStack.ElementAt(1);
                            property.SetValue(command, value, null);
                            argumentsStack.RemoveAll(a => a == value);
                        } else
                        {
                            _loggerService.LogError($"Valor inválid para a opção --{property.Name.ToLower()}.");
                            return default;
                        }
                    }

                    argumentsStack.RemoveAll(a => matchArguments.Contains(a));
                    continue;
                }

                if (propertyAttribute is not null)
                {
                    var existIndex = argumentsStack.Count >= 1;
                    var validArgument = existIndex && (propertyAttribute.IsCumulative 
                        ? true : !argumentsStack.ElementAt(0).StartsWith("-")
                    );
                    if (validArgument)
                    {
                        if (propertyAttribute.IsCumulative)
                        {
                            var values = argumentsStack.GetRange(0, argumentsStack.Count);
                            property.SetValue(command, string.Join(" ", values), null);
                            argumentsStack.RemoveAll(a => values.Contains(a));
                        } else
                        {
                            var value = argumentsStack.ElementAt(0);
                            property.SetValue(command, value, null);
                            argumentsStack.RemoveAll(a => a == value);
                        }
                    }
                    else if (propertyAttribute.IsRequired)
                    {
                        _loggerService.LogError($"Parâmetro obrigatório <{property.Name.ToSnakeCase().ToUpper()}> não fornecido.");
                        return default;
                    }
                }
            }

            if (argumentsStack.Any())
            {
                _loggerService.LogError($"Argumento(s) não reconhecido(s) {string.Join(", ", argumentsStack)}.");
                return default;
            }

            return command;
        }
    }
}
