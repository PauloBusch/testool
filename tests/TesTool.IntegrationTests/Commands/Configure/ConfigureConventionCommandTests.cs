using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Commands.Configure;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.IntegrationTests._Common;
using Xunit;

namespace TesTool.IntegrationTests.Commands.Configure
{
    public class ConfigureConventionCommandTests : TestBase
    {
        private readonly ICommandFactoryService _factory;
        private readonly ISettingInfraService _settingsService;

        public ConfigureConventionCommandTests(TesToolFixture fixture) : base(fixture)
        {
            _factory = _services.GetRequiredService<ICommandFactoryService>();
            _settingsService = _services.GetRequiredService<ISettingInfraService>();
        }

        [Theory]
        [InlineData(@"c c convention.json")]
        [InlineData(@"configure convention convention.json")]
        public async Task ShouldRejectConfigureConventionAsync(string rawCommand)
        {
            var arguments = rawCommand.Split(" ");
            var expectedPath = arguments.Last();
            var command = _factory.CreateCommand(arguments) as ConfigureConventionCommand;

            await command.ExecuteAsync();

            Assert.NotEqual(expectedPath, await _settingsService.GetStringAsync(SettingEnumerator.CONVENTION_PATH_FILE.Key));
            _loggerServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(@"c c Assets\convention.json")]
        [InlineData(@"configure convention Assets\convention.json")]
        public async Task ShouldAcceptConfigureConventionAsync(string rawCommand)
        {
            var arguments = rawCommand.Split(" ");
            var expectedPath = arguments.Last();
            var command = _factory.CreateCommand(arguments) as ConfigureConventionCommand;

            await command.ExecuteAsync();

            Assert.Equal(expectedPath, await _settingsService.GetStringAsync(SettingEnumerator.CONVENTION_PATH_FILE.Key));
            _loggerServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Never);
        }
    }
}
