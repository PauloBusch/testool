using System;
using System.Threading.Tasks;
using TesTool.Core.Commands.Configure;
using TesTool.Core.Interfaces.Services;
using TesTool.IntegrationTests.Common;
using Xunit;

namespace TesTool.IntegrationTests.Services
{
    public class ArgumentsServiceTests : ServiceTestBase<IArgumentsService>
    {
        public ArgumentsServiceTests(TesToolFixture fixture) : base(fixture) { }

        [Theory]
        [InlineData("move")]
        [InlineData("create")]
        [InlineData("m c u")]
        public async Task ShouldRejectCommandAsync(string rawArguments)
        {
            var arguments = rawArguments.Split(" ");

            var command = await _service.GetCommandAsync(arguments);

            Assert.Null(command);
        }

        [Theory]
        [InlineData(@"--configure --convention C:\Projetos\Tests\convention.json", typeof(ConfigureConventionCommand))]
        [InlineData(@"--configure --project C:\Projetos\Api", typeof(ConfigureProjectCommand))]
        public async Task ShouldReturnCommandInstanceAsync(string rawArguments, Type type)
        {
            var arguments = rawArguments.Split(" ");

            var command = await _service.GetCommandAsync(arguments);

            Assert.NotNull(command);
            Assert.Equal(type, command.GetType());
        }

        [Fact]
        public async Task ShouldReturnCommandWithParametersAsync()
        {
            var directory = @"C:\Projetos\Api";
            var arguments = new [] { "--configure", "--project", directory };

            var command = await _service.GetCommandAsync(arguments);
            var configureProjectCommand = command as ConfigureProjectCommand;

            Assert.NotNull(configureProjectCommand);
            Assert.Equal(directory, configureProjectCommand.Directory);
        }
    }
}
