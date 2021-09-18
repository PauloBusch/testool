using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class CommandFactoryService : ICommandFactoryService
    {
        private readonly ILogger<CommandFactoryService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CommandFactoryService(
            ILogger<CommandFactoryService> logger,
            IServiceProvider serviceProvider
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public ICommand CreateCommand(string[] args)
        {
            if (args == null || !args.Any())
            {
                _logger.LogError("Require any argument.");
                return default;
            }

            var commandTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => typeof(ICommand).IsAssignableFrom(t))
                .ToList();

            var commandType = commandTypes.SingleOrDefault(type => {
                var commands = type.GetCustomAttributes<CommandAttribute>().Reverse();
                if (args.Length < commands.Count()) return false;
                return commands
                    .Select((c, i) => new { Command = c, Index = i })
                    .All(p => new [] { p.Command.Name, p.Command.Alias }
                        .Contains(args.ElementAt(p.Index))
                    );
            });
            if (commandType == null)
            {
                _logger.LogError("Command not found.");
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

                var matchArguments = new [] { 
                    $"--{property.Name.ToLower()}", 
                    $"-{property.Name.ToLower().First()}" 
                };
                if (!arguments.Any(a => matchArguments.Contains(a))) {
                    if (flagAttribute != null) continue;
                    if (propertyAttribute != null 
                        && propertyAttribute.IsRequired
                        && !propertyAttribute.IsDefault 
                    )
                    {
                        _logger.LogError($"Property {property.Name} is required.");
                        return default;
                    }
                }

                if (flagAttribute != null) property.SetValue(command, true, null);
                if (propertyAttribute != null)
                {
                    var valueIndex = propertyAttribute.IsDefault ? 0 : 1;
                    var existIndex = arguments.Count >= valueIndex + 1;
                    var validArgument = existIndex && !arguments.ElementAt(valueIndex).StartsWith("-");
                    if (validArgument)
                    {
                        var value = arguments.ElementAt(valueIndex);
                        property.SetValue(command, value, null);
                        arguments.RemoveAll(a => a == value);
                    } else if (propertyAttribute.IsRequired)
                    {
                        _logger.LogError($"No value provied from {property.Name} property.");
                        return default;
                    }
                }
                arguments.RemoveAll(a => matchArguments.Contains(a));
            }

            if (arguments.Any())
            {
                _logger.LogError($"Unrecognized arguments {string.Join(", ", arguments)}.");
                return default;
            }

            return command;
        }
    }
}
