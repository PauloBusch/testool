using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using TesTool.Cli;
using TesTool.Core.Interfaces.Services;
using Xunit;

namespace TesTool.IntegrationTests._Common
{
    public class TestBase : IClassFixture<TesToolFixture>
    {
        protected readonly IServiceProvider _services;
        protected readonly Mock<ILoggerInfraService> _loggerServiceMock;

        public TestBase(TesToolFixture _) {
            _loggerServiceMock = new Mock<ILoggerInfraService>();
            _services = new ServiceCollection()
                .AddServices()
                .AddSingleton(typeof(ILoggerInfraService), _loggerServiceMock.Object)
                .BuildServiceProvider();
        }
    }
}
