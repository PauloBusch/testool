using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Services
{
    public class CommandExplorerService : ICommandExplorerService
    {
        private static IEnumerable<Type> _commandTypes;

        public IEnumerable<Type> GetAllCommandTypes()
        {
            if (_commandTypes is not null) return _commandTypes;

            _commandTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => typeof(ICommand).IsAssignableFrom(t))
                .ToList();

            return _commandTypes;
        }

        public Type GetCommandTypeExact(string[] arguments)
        {
            if (arguments is null || arguments.Length == 0) return default;
            foreach (var type in GetAllCommandTypes())
            {
                var isDefault = type.GetCustomAttributes<DefaultAttribute>().Any();
                var commands = type.GetCustomAttributes<CommandAttribute>().Reverse();
                if (isDefault && commands.Any(c => arguments.Any(a => c.Equals(a)))) return type;
                if (arguments.Length < commands.Count()) continue;
                
                var matched = commands
                    .Select((c, i) => new { Command = c, Index = i })
                    .All(props => props.Command.Equals(arguments.ElementAt(props.Index)));
                if (matched) return type;
            }
            return default;
        }

        public IEnumerable<Type> GetCommandTypesMatched(string[] arguments)
        {
            if (arguments is null || arguments.Length == 0) return default;
            return GetAllCommandTypes().Where(type =>
            {
                var isDefault = type.GetCustomAttributes<DefaultAttribute>().Any();
                var commands = type.GetCustomAttributes<CommandAttribute>().Reverse();
                if (isDefault && commands.Any(c => arguments.Any(a => c.Equals(a)))) return true;
                return commands
                    .Select((c, i) => new { Command = c, Index = i })
                    .All(props => arguments.Length > props.Index ? props.Command.Equals(arguments.ElementAt(props.Index)) : true);
            }).ToArray();
        }
    }
}
