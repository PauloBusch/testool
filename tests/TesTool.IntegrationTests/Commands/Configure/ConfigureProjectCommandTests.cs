﻿using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Commands.Configure;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.IntegrationTests._Common;
using Xunit;

namespace TesTool.IntegrationTests.Commands.Configure
{
    public class ConfigureProjectCommandTests : TestBase
    {
        private readonly ICommandFactoryService _factory;
        private readonly ISettingInfraService _settingsService;

        public ConfigureProjectCommandTests(TesToolFixture fixture) : base(fixture)
        {
            _factory = _services.GetRequiredService<ICommandFactoryService>();
            _settingsService = _services.GetRequiredService<ISettingInfraService>();
        }

        [Theory]
        [InlineData(@"c p Project")]
        [InlineData(@"c p sample.csproj")]
        [InlineData(@"configure project Project")]
        public async Task ShouldRejectConfigureProjectAsync(string rawCommand)
        {
            var arguments = rawCommand.Split(" ");
            var expectedPath = arguments.Last();
            var command = _factory.CreateCommand(arguments) as ConfigureWebApiProjectCommand;

            await command.ExecuteAsync(new CommandContext(false));

            Assert.NotEqual(expectedPath, _settingsService.ProjectWebApiDirectory);
            _loggerServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(@"c p Assets")]
        [InlineData(@"c p Assets/sample.csproj")]
        [InlineData(@"configure project Assets")]
        public async Task ShouldAcceptConfigureProjectAsync(string rawCommand)
        {
            var command = _factory.CreateCommand(rawCommand.Split(" ")) as ConfigureWebApiProjectCommand;

            await command.ExecuteAsync(new CommandContext(false));

            Assert.Equal(@"Assets\sample.csproj", _settingsService.ProjectWebApiDirectory);
            _loggerServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Never);
        }
    }
}
