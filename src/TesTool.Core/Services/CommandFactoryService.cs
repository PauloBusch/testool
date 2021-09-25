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

        public ICommand CreateCommand(string[] args)
        {
            if (args == null || !args.Any() || args.All(a => string.IsNullOrWhiteSpace(a)))
            {
                var defaultCommandType = _commandExplorerService.GetDefaultCommandType();
                if (defaultCommandType != null) return _serviceProvider.GetService(defaultCommandType) as ICommand;
                _loggerService.LogError("Nenhum parâmetro foi informado.");
                return default;
            }

            var commandTypesMatched = _commandExplorerService.GetCommandTypesMatched(args);
            if (!commandTypesMatched.Any())
            {
                _loggerService.LogError("Comando não encontrado.");
                _loggerService.LogInformation("Execute 'testool --help' para ver os comandos válidos.");
                return default;
            }

            var commandType = _commandExplorerService.GetCommandTypeExact(args);
            if (commandType == null)
            {
                _loggerService.LogError("Comando incompleto.");
                _loggerService.LogInformation("Execute 'testool --help [command]' para ver mais informações sobre o comando.");
                return default;
            }

            var command = _serviceProvider.GetService(commandType) as ICommand;
            var properties = commandType.GetProperties();
            var arguments = new List<string>(args);
            arguments.RemoveRange(0, commandType.GetCustomAttributes<CommandAttribute>().Count());
            foreach (var property in properties)
            {
                var flagAttribute = property.GetCustomAttribute<FlagAttribute>();
                var propertyAttribute = property.GetCustomAttribute<ParameterAttribute>();
                if (flagAttribute == null && propertyAttribute == null) continue;

                var matchArguments = new[] {
                    $"--{property.Name.ToLower()}",
                    $"-{property.Name.ToLower().First()}"
                };
                if (!arguments.Any(a => matchArguments.Contains(a)))
                {
                    if (flagAttribute != null) continue;
                    if (propertyAttribute != null
                        && propertyAttribute.IsRequired
                        && !propertyAttribute.IsDefault
                    )
                    {
                        _loggerService.LogError($"Parâmetro obrigatório <{property.Name.ToSnakeCase().ToUpper()}>.");
                        return default;
                    }
                }

                if (flagAttribute != null) property.SetValue(command, true, null);
                if (propertyAttribute != null)
                {
                    var valueIndex = propertyAttribute.IsDefault ? 0 : 1;
                    var existIndex = arguments.Count >= valueIndex + 1;
                    var validArgument = existIndex && (propertyAttribute.IsCumulative 
                        ? true : !arguments.ElementAt(valueIndex).StartsWith("-")
                    );
                    if (validArgument)
                    {
                        if (propertyAttribute.IsCumulative)
                        {
                            var values = arguments.GetRange(valueIndex, arguments.Count - valueIndex);
                            property.SetValue(command, string.Join(" ", values), null);
                            arguments.RemoveAll(a => values.Contains(a));
                        } else
                        {
                            var value = arguments.ElementAt(valueIndex);
                            property.SetValue(command, value, null);
                            arguments.RemoveAll(a => a == value);
                        }
                    }
                    else if (propertyAttribute.IsRequired)
                    {
                        _loggerService.LogError($"Nenhum valor foi fornecido para o parâmetro <{property.Name.ToSnakeCase().ToUpper()}>.");
                        return default;
                    }
                }
                arguments.RemoveAll(a => matchArguments.Contains(a));
            }

            if (arguments.Any())
            {
                _loggerService.LogError($"Argumento(s) não reconhecido(s) {string.Join(", ", arguments)}.");
                return default;
            }

            return command;
        }
    }
}
