using System;
using System.Linq;
using System.Reflection;
using TesTool.Core.Attributes;
using TesTool.Core.Commands.Configure;

namespace TesTool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var command = new ConfigureProjectCommand();

            var commands = command.GetType().GetCustomAttributes<CommandAttribute>();

            Console.WriteLine(commands.Select(c => $"Name: {c.Name}, Alias: {c.Alias}, TypeId: {c.TypeId}"));
        }
    }
}
