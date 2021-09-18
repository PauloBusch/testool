using Microsoft.Extensions.DependencyInjection;
using TesTool.Core.Interfaces.Services;
using TesTool.Infra.Services;

namespace TesTool.Cli
{
    public static class TesToolDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<ISettingsService, SettingsService>()
                .AddSingleton<ICommandFactoryService, CommandFactoryService>();
        }
    }
}
