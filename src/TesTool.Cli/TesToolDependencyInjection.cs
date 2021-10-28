using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TesTool.Core.Interfaces;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Interfaces.Services.Factories;
using TesTool.Core.Interfaces.Services.Fakers;
using TesTool.Core.Services;
using TesTool.Core.Services.Endpoints;
using TesTool.Core.Services.Factories;
using TesTool.Core.Services.Fakers;
using TesTool.Core.Services.Handlers;
using TesTool.Infra.Services;

namespace TesTool.Cli
{
    public static class TesToolDependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddSingleton<IServiceResolver, ServiceResolver>()
                .AddSingleton<ILoggerInfraService, LoggerInfraService>()
                .AddSingleton<ISettingInfraService, SettingsInfraService>()
                .AddSingleton<IEnvironmentInfraService, EnvironmentInfraService>()
                .AddSingleton<IFileSystemInfraService, FileSystemInfraService>()
                .AddSingleton<ITemplateCodeInfraService, TemplateCodeInfraService>()
                .AddSingleton<IConventionInfraService, ConventionInfraService>()
                .AddSingleton<IExpressionInfraService, ExpressionInfraService>()
                .AddSingleton<ISerializerInfraService, SerializerInfraService>()

                .AddSingleton<IWebApiScanInfraService, WebApiScanInfraService>()
                .AddSingleton<IWebApiDbContextInfraService, WebApiDbContextInfraService>()
                .AddSingleton<ITestScanInfraService, TestScanInfraService>()
                .AddSingleton<ITestCodeInfraService, TestCodeInfraService>()

                .AddSingleton<IFakeModelService, FakeModelService>()
                .AddSingleton<IFakeEntityService, FakeEntityService>()
                .AddSingleton<ICompareService, CompareService>()
                .AddSingleton<IControllerService, ControllerService>()

                .AddSingleton<IGetOneEndpointTestService, GetOneEndpointTestService>()
                .AddSingleton<IGetListEndpointTestService, GetListEndpointTestService>()
                .AddSingleton<IPostEndpointTestService, PostEndpointTestService>()
                .AddSingleton<IPutEndpointTestService, PutEndpointTestService>()
                .AddSingleton<IDeleteEndpointTestService, DeleteEndpointTestService>()

                .AddSingleton<IFactoryModelService, FactoryModelService>()
                .AddSingleton<IFactoryEntityService, FactoryEntityService>()
                .AddSingleton<IFactoryCompareService, FactoryCompareService>()

                .AddSingleton<ICommandHandler, CommandHandler>()
                .AddSingleton<ICommandExplorerService, CommandExplorerService>()
                .AddSingleton<ICommandFactoryService, CommandFactoryService>()
                .AddSingleton<ISolutionService, SolutionService>();

            var commandTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => typeof(ICommand).IsAssignableFrom(t))
                .ToList();
            commandTypes.ForEach(c => services.AddTransient(c));
        
            return services;
        }
    }
}
