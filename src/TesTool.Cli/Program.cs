using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TesTool.Core.Exceptions;
using TesTool.Core.Extensions;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync(args).Wait();
        }

        static async Task RunAsync(string[] args)
        {
            try
            {
                using var serviceProvider = new ServiceCollection()
                    .AddServices()
                    .BuildServiceProvider();
                
                var argumentsCoreService = serviceProvider.GetService<ICommandFactoryService>();
                var commandHandler = serviceProvider.GetService<ICommandHandler>();
                var command = argumentsCoreService.CreateCommand(args);
                if (command is not null) await commandHandler.HandleAsync(command, cascade: false);
            }
            catch (TesToolExceptionBase exception)
            {
                Console.WriteLine(exception.GetFullMessageString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.GetFullMessageString());
            }
        }
    }
}
