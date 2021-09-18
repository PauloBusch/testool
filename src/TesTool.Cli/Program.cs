using System;
using System.Linq;
using System.Reflection;
using TesTool.Core.Attributes;
using TesTool.Core.Commands.Configure;
using TesTool.Infra.Service;

namespace TesTool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var command = new ConfigureProjectCommand();

            var commands = command.GetType().GetCustomAttributes<CommandAttribute>();

            Console.WriteLine(commands.Select(c => $"Name: {c.Name}, Alias: {c.Alias}, TypeId: {c.TypeId}"));

            var settings = new SettingsInfraService();
            settings.SetStringAsync("PROJECT_DIRECTORY", "C:/Projetos").Wait();
            var test = settings.GetStringAsync("PROJECT_DIRECTORY").Result;
        }
    }
}
