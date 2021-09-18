using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(options => options.AddConsole())
                .AddServices()
                .BuildServiceProvider();

            var argumentsCoreService = serviceProvider.GetService<ICommandFactoryService>();
            argumentsCoreService.CreateCommandAsync(args).Result?.ExecuteAsync().Wait();
        }
    }
}
