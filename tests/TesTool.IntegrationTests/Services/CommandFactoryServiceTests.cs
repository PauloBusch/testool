using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using TesTool.Core.Commands.Configure;
using TesTool.Infra.Services;
using TesTool.IntegrationTests.Common;
using Xunit;

namespace TesTool.IntegrationTests.Services
{
    public class CommandFactoryServiceTests : TestBase
    {
        private readonly Mock<ILogger<CommandFactoryService>> _loggerMock;
        private readonly CommandFactoryService _service;

        public CommandFactoryServiceTests(TesToolFixture fixture) : base(fixture) { 
            _loggerMock = new Mock<ILogger<CommandFactoryService>>();
            _service = new CommandFactoryService(_loggerMock.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("move")]
        [InlineData("create")]
        [InlineData("m c u")]
        public async Task ShouldRejectCommandAsync(string rawArguments)
        {
            var arguments = rawArguments.Split(" ");

            var command = await _service.CreateCommandAsync(arguments);

            Assert.Null(command);
            _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(@"--configure --convention C:\Projetos\Tests\convention.json", typeof(ConfigureConventionCommand))]
        [InlineData(@"--configure --project C:\Projetos\Api", typeof(ConfigureProjectCommand))]
        public async Task ShouldReturnCommandInstanceAsync(string rawArguments, Type type)
        {
            var arguments = rawArguments.Split(" ");

            var command = await _service.CreateCommandAsync(arguments);

            Assert.NotNull(command);
            Assert.Equal(type, command.GetType());
            _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ShouldReturnCommandWithParametersAsync()
        {
            var directory = @"C:\Projetos\Api";
            var arguments = new [] { "--configure", "--project", directory };

            var command = await _service.CreateCommandAsync(arguments);
            var configureProjectCommand = command as ConfigureProjectCommand;

            Assert.NotNull(configureProjectCommand);
            Assert.Equal(directory, configureProjectCommand.Directory);
            _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Never);
        }
    }
}
