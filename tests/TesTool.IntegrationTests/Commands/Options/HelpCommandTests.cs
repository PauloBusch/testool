﻿using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;
using TesTool.Core.Commands.Help;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.IntegrationTests._Common;
using Xunit;

namespace TesTool.IntegrationTests.Commands.Options
{
    public class HelpCommandTests : TestBase
    {
        private readonly ICommandFactoryService _factory;

        public HelpCommandTests(TesToolFixture _) : base(_)
        {
            _factory = _services.GetRequiredService<ICommandFactoryService>();
        }

        [Theory]
        [InlineData("-h o")]
        [InlineData("-h p")]
        [InlineData("-h s")]
        [InlineData("--help test")]
        [InlineData("--help configure projet")]
        [InlineData("--help generate code")]
        public async Task ShouldRejectHelpTextAsync(string rawCommand)
        {
            var arguments = rawCommand.Split(" ");
            var command = _factory.CreateCommand(arguments) as HelpCommand;
            Assert.NotNull(command);

            await command.ExecuteAsync(new CommandContext(false));

            _loggerServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData("-h g")]
        [InlineData("-h c")]
        [InlineData("-h g c")]
        [InlineData("-h c p")]
        [InlineData("-h c c")]
        [InlineData("g -h")]
        [InlineData("c -h")]
        [InlineData("g c -h")]
        [InlineData("c p -h")]
        [InlineData("c c -h")]
        [InlineData("--help generate")]
        [InlineData("--help configure")]
        [InlineData("--help generate controller")]
        [InlineData("--help configure project")]
        [InlineData("--help configure convention")]
        [InlineData("generate --help")]
        [InlineData("configure --help")]
        [InlineData("generate controller --help")]
        [InlineData("configure project --help")]
        [InlineData("configure convention --help")]
        public async Task ShouldLogHelpTextAsync(string rawCommand)
        {
            var command = _factory.CreateCommand(rawCommand.Split(" ")) as HelpCommand;
            Assert.NotNull(command);

            await command.ExecuteAsync(new CommandContext(false));

            _loggerServiceMock.Verify(l => l.LogInformation(It.IsAny<string>()), Times.AtLeastOnce);
            _loggerServiceMock.Verify(l => l.LogWarning(It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Never);
        }
    }
}
