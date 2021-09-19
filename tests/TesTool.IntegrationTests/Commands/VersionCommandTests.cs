using Moq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TesTool.Core.Commands.Options;
using TesTool.Core.Interfaces;
using TesTool.IntegrationTests._Common;
using Xunit;

namespace TesTool.IntegrationTests.Commands
{
    public class VersionCommandTests : TestBase
    {
        public VersionCommandTests(TesToolFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ShouldLogVersionAsync()
        {
            var loggerServiceMock = new Mock<ILoggerService<VersionCommand>>();
            var command = new VersionCommand(loggerServiceMock.Object);

            await command.ExecuteAsync();

            var regex = new Regex(@"^\d+.\d+.\d+.\d+$");
            loggerServiceMock.Verify(l => l.LogInformation(It.Is<string>(v => regex.IsMatch(v))), Times.Once);
        }
    }
}
