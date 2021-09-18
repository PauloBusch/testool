using Moq;
using System.Threading.Tasks;
using TesTool.Core.Commands.Configure;
using TesTool.Core.Interfaces.Services;
using TesTool.UnitTests.Common;
using Xunit;

namespace TesTool.UnitTests.Commands
{
    public class ConfigureProjectCommandTests : TestBase
    {
        public ConfigureProjectCommandTests(TesToolFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ShouldConfigureProjectAsync()
        {
            var settingsServiceMock = new Mock<ISettingsService>();

            var command = new ConfigureProjectCommand(settingsServiceMock.Object) { Directory = @"C:\Projects" };

            await command.ExecuteAsync();

            settingsServiceMock.Verify(s => s.SetStringAsync(ConfigureCommandBase.SETTINGS_KEY, command.Directory), Times.Once);
        }
    }
}
