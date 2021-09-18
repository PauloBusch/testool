using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class CommandFactoryService : ICommandFactoryService
    {
        private readonly ILogger<CommandFactoryService> _logger;

        public CommandFactoryService(ILogger<CommandFactoryService> logger)
        {
            _logger = logger;
        }

        public async Task<ICommand> CreateCommandAsync(string[] args)
        {
            if (args == null || !args.Any())
            {
                _logger.LogError("Require any argument");
                return default;
            }

            var commandType = typeof(ICommand);
            var commandTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => commandType.IsAssignableFrom(t))
                .ToList();

            var argumentTypes = args.Select(a => new KeyValuePair<string, Type>(a.Trim(), default));
            var commands = commandTypes.SelectMany(t => t.GetCustomAttributes<CommandAttribute>().Reverse());

            return default;
        }
    }
}
