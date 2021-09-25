using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Services;
using TesTool.Infra.Services;

namespace TesTool.Cli
{
    public static class TesToolDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddSingleton<ILoggerInfraService, LoggerInfraService>()
                .AddSingleton<ISettingInfraService, SettingsInfraService>()
                .AddSingleton<ICommandExplorerService, CommandExplorerService>()
                .AddSingleton<ICommandFactoryService, CommandFactoryService>();

            var commandTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => typeof(ICommand).IsAssignableFrom(t))
                .ToList();
            commandTypes.ForEach(c => services.AddScoped(c));
        
            return services;
        }
    }
}
