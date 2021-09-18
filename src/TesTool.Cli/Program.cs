using Microsoft.Extensions.DependencyInjection;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddServices()
                .BuildServiceProvider();

            var argumentsCoreService = serviceProvider.GetService<IArgumentsService>();
            argumentsCoreService.GetCommandAsync(args).Result?.ExecuteAsync().Wait();
        }
    }
}
