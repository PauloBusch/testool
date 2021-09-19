using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using TesTool.Cli;
using TesTool.Core.Interfaces;
using Xunit;

namespace TesTool.IntegrationTests._Common
{
    public class TestBase : IClassFixture<TesToolFixture>
    {
        protected readonly IServiceProvider _services;
        protected readonly Mock<ILoggerService> _loggerServiceMock;

        public TestBase(TesToolFixture _) {
            _loggerServiceMock = new Mock<ILoggerService>();
            _services = new ServiceCollection()
                .AddServices()
                .AddSingleton(typeof(ILoggerService), _loggerServiceMock.Object)
                .BuildServiceProvider();
        }
    }
}
