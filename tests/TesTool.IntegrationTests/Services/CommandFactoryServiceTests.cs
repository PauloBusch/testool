using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using TesTool.Core.Commands.Configure;
using TesTool.Core.Commands.Generate;
using TesTool.Core.Commands.Help;
using TesTool.Core.Interfaces.Services;
using TesTool.IntegrationTests._Common;
using Xunit;

namespace TesTool.IntegrationTests.Services
{
    public class CommandFactoryServiceTests : TestBase
    {
        private readonly ICommandFactoryService _factory;

        public CommandFactoryServiceTests(TesToolFixture fixture) : base(fixture) { 
            _factory = _services.GetRequiredService<ICommandFactoryService>();
        }

        [Theory]
        [InlineData("test")]
        [InlineData("--move")]
        [InlineData("--create")]
        [InlineData("c -c")]
        [InlineData("m c u")]
        [InlineData("-c c path reject")]
        public void ShouldRejectCommand(string rawArguments)
        {
            var arguments = rawArguments?.Split(" ");

            var command = _factory.CreateCommand(arguments);

            Assert.Null(command);
            _loggerServiceMock.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldReturnDefaultCommand(string rawArguments)
        {
            var arguments = rawArguments?.Split(" ");

            var command = _factory.CreateCommand(arguments);

            Assert.NotNull(command);
            Assert.Equal(typeof(HelpCommand), command.GetType());
            _loggerServiceMock.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData(@"c c C:\Projetos\Tests\convention.json", typeof(ConfigureConventionCommand))]
        [InlineData(@"c p C:\Projetos\Api", typeof(ConfigureProjectCommand))]
        [InlineData(@"configure convention C:\Projetos\Tests\convention.json", typeof(ConfigureConventionCommand))]
        [InlineData(@"configure project C:\Projetos\Api", typeof(ConfigureProjectCommand))]
        public void ShouldReturnCommandInstance(string rawArguments, Type type)
        {
            var arguments = rawArguments.Split(" ");

            var command = _factory.CreateCommand(arguments);

            Assert.NotNull(command);
            Assert.Equal(type, command.GetType());
            _loggerServiceMock.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("-s")]
        [InlineData("--static")]
        public void ShouldReturnCommandWithFlags(string flag)
        {
            var arguments = new[] { "generate", "project", "Project", flag };

            var command = _factory.CreateCommand(arguments.ToArray());
            var configureProjectCommand = command as GenerateProjectCommand;

            Assert.NotNull(configureProjectCommand);
            Assert.True(configureProjectCommand.Static);
            _loggerServiceMock.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("-o")]
        [InlineData("--output")]
        public void ShouldReturnCommandWithExplicitParameters(string parameter)
        {
            var output = @"C:\Projetos\Tests";
            var arguments = new [] { 
                "generate", "project", "Project",
                parameter, output 
            };

            var command = _factory.CreateCommand(arguments.ToArray());
            var configureProjectCommand = command as GenerateProjectCommand;

            Assert.NotNull(configureProjectCommand);
            Assert.Equal(output, configureProjectCommand.Output);
            _loggerServiceMock.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void ShouldReturnCommandWithImplicitParameters()
        {
            var sourceClassName = "ContactRequest";
            var targetClassName = "ContactResponse";
            var comparatorName = "ObjectComparator";
            var arguments = new [] { 
                "generate", "equal", 
                sourceClassName, targetClassName, comparatorName 
            };

            var command = _factory.CreateCommand(arguments.ToArray());
            var configureProjectCommand = command as GenerateCompareCommand;

            Assert.NotNull(configureProjectCommand);
            Assert.Equal(sourceClassName, configureProjectCommand.SourceClassName);
            Assert.Equal(targetClassName, configureProjectCommand.TargetClassName);
            Assert.Equal(comparatorName, configureProjectCommand.ComparatorName);
            _loggerServiceMock.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Never);
        }
    }
}
