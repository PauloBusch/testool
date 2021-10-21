using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Interfaces.Services.Fakers;
using TesTool.Core.Services;
using TesTool.Core.Services.Factories;
using TesTool.Core.Services.Fakers;
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
                .AddSingleton<IEnvironmentInfraService, EnvironmentInfraService>()
                .AddSingleton<IFileSystemInfraService, FileSystemInfraService>()
                .AddSingleton<ITemplateCodeInfraService, TemplateCodeInfraService>()
                .AddSingleton<IConventionInfraService, ConventionInfraService>()
                .AddSingleton<IExpressionInfraService, ExpressionInfraService>()
                .AddSingleton<ISerializerInfraService, SerializerInfraService>()

                .AddSingleton<IWebApiScanInfraService, WebApiScanInfraService>()
                .AddSingleton<ITestScanInfraService, TestScanInfraService>()
                .AddSingleton<ITestCodeInfraService, TestCodeInfraService>()

                .AddSingleton<IFakeEntityService, FakeEntityService>()
                .AddSingleton<ICompareService, CompareService>()

                .AddSingleton<IFactoryModelService, FactoryModelService>()
                .AddSingleton<IFactoryEntityService, FactoryEntityService>()
                .AddSingleton<IFactoryCompareService, FactoryCompareService>()

                .AddSingleton<ISolutionService, SolutionService>()
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
