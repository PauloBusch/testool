using Moq;
using System.Threading.Tasks;
using TesTool.Core.Commands.Help;
using TesTool.Core.Interfaces.Services;
using TesTool.UnitTests._Common;
using Xunit;

namespace TesTool.UnitTests.Commands.Options
{
    public class HelpCommandTests : TestBase
    {
        private readonly Mock<ILoggerInfraService> _loggerServiceMock;
        private readonly Mock<ICommandExplorerService> _commandExplorerServiceMock;

        public HelpCommandTests(TesToolFixture fixture) : base(fixture) {
            _loggerServiceMock = new Mock<ILoggerInfraService>();
            _commandExplorerServiceMock = new Mock<ICommandExplorerService>();
            _commandExplorerServiceMock
                .Setup(c => c.GetAllCommandTypes())
                .Returns(new[] { typeof(HelpCommand) });
        }

        [Theory]
        [InlineData("t")]
        [InlineData("g t")]
        [InlineData("t e")]
        public async Task ShouldRejectHelpTextAsync(string parameter)
        {
            var command = new HelpCommand(_commandExplorerServiceMock.Object, _loggerServiceMock.Object) { Command = parameter };

            await command.ExecuteAsync();

            _loggerServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("g c")]
        [InlineData("g e")]
        public async Task ShouldLogHelpTextAsync(string parameter)
        {
            var command = new HelpCommand(_commandExplorerServiceMock.Object, _loggerServiceMock.Object){ Command = parameter };

            await command.ExecuteAsync();
            
            _loggerServiceMock.Verify(l => l.LogInformation(It.IsAny<string>()), Times.AtLeastOnce);
            _loggerServiceMock.Verify(l => l.LogWarning(It.IsAny<string>()), Times.Never);
            _loggerServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Never);
        }
    }
}
