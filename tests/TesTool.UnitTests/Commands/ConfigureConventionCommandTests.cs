using Moq;
using System.Threading.Tasks;
using TesTool.Core.Commands.Configure;
using TesTool.Core.Interfaces.Services;
using TesTool.UnitTests.Common;
using Xunit;

namespace TesTool.UnitTests.Commands
{
    public class ConfigureConventionCommandTests : TestBase
    {
        public ConfigureConventionCommandTests(TesToolFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ShouldConfigureConventionAsync()
        {
            var settingsServiceMock = new Mock<ISettingsService>();

            var command = new ConfigureConventionCommand(settingsServiceMock.Object) { Directory = @"C:\Projects\convention.json" };

            await command.ExecuteAsync();

            settingsServiceMock.Verify(s => s.SetStringAsync(ConfigureCommandBase.SETTINGS_KEY, command.Directory), Times.Once);
        }
    }
}
