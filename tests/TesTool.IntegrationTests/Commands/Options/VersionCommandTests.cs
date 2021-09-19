using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TesTool.Core.Commands.Options;
using TesTool.IntegrationTests._Common;
using Xunit;

namespace TesTool.IntegrationTests.Commands
{
    public class VersionCommandTests : TestBase
    {
        private readonly VersionCommand _command;

        public VersionCommandTests(TesToolFixture fixture) : base(fixture) { 
            _command = _services.GetRequiredService<VersionCommand>();    
        }

        [Fact]
        public async Task ShouldLogVersionAsync()
        {
            await _command.ExecuteAsync();

            var regex = new Regex(@"^\d+.\d+.\d+.\d+$");
            _loggerServiceMock.Verify(l => l.LogInformation(It.Is<string>(v => regex.IsMatch(v))), Times.Once);
        }
    }
}
