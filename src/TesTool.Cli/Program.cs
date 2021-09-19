using Microsoft.Extensions.DependencyInjection;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            using var serviceProvider = new ServiceCollection()
                .AddServices()
                .BuildServiceProvider();

            var argumentsCoreService = serviceProvider.GetService<ICommandFactoryService>();
            argumentsCoreService.CreateCommand(args)?.ExecuteAsync().Wait();
        }
    }
}
