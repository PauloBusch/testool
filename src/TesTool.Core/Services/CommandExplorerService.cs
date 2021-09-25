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

        public Type GetCommandTypeExact(string[] args)
        {
            if (args is null || args.Length == 0) return default;
            return GetAllCommandTypes().SingleOrDefault(type =>
            {
                var commands = type.GetCustomAttributes<CommandAttribute>().Reverse();
                if (args.Length < commands.Count()) return false;
                return commands
                    .Select((c, i) => new { Command = c, Index = i })
                    .All(props => props.Command.Equals(args.ElementAt(props.Index)));
            });
        }

        public IEnumerable<Type> GetCommandTypesMatched(string[] args)
        {
            if (args is null || args.Length == 0) return default;
            return GetAllCommandTypes().Where(type =>
            {
                var commands = type.GetCustomAttributes<CommandAttribute>().Reverse();
                return commands
                    .Select((c, i) => new { Command = c, Index = i })
                    .All(props => args.Length > props.Index ? props.Command.Equals(args.ElementAt(props.Index)) : true);
            }).ToArray();
        }

        public Type GetDefaultCommandType()
        {
            return GetAllCommandTypes().SingleOrDefault(type =>
            {
                var commands = type.GetCustomAttributes<CommandAttribute>().Reverse();
                return commands.Any(c => c.IsDefault);
            });
        }
    }
}
