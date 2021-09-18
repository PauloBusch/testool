using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using TesTool.Cli;
using TesTool.Core.Commands.Configure;
using TesTool.Core.Commands.Generate;
using TesTool.Infra.Services;
using TesTool.IntegrationTests.Common;
using Xunit;

namespace TesTool.IntegrationTests.Services
{
    public class CommandFactoryServiceTests : TestBase
    {
        private readonly Mock<ILogger<CommandFactoryService>> _loggerMock;
        private readonly CommandFactoryService _service;

        public CommandFactoryServiceTests(TesToolFixture fixture) : base(fixture) { 
            var serviceProvider = new ServiceCollection().AddServices().BuildServiceProvider();
            _loggerMock = new Mock<ILogger<CommandFactoryService>>();
            _service = new CommandFactoryService(_loggerMock.Object, serviceProvider);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("test")]
        [InlineData("--move")]
        [InlineData("--create")]
        [InlineData("-m -c -u")]
        [InlineData("-c -c")]
        [InlineData("-c -c path reject")]
        public void ShouldRejectCommand(string rawArguments)
        {
            var arguments = rawArguments?.Split(" ");

            var command = _service.CreateCommand(arguments);

            Assert.Null(command);
            _loggerMock.Verify(logger => logger.Log
                (
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Theory]
        [InlineData(@"-c -c C:\Projetos\Tests\convention.json", typeof(ConfigureConventionCommand))]
        [InlineData(@"-c -p C:\Projetos\Api", typeof(ConfigureProjectCommand))]
        [InlineData(@"--configure --convention C:\Projetos\Tests\convention.json", typeof(ConfigureConventionCommand))]
        [InlineData(@"--configure --project C:\Projetos\Api", typeof(ConfigureProjectCommand))]
        public void ShouldReturnCommandInstance(string rawArguments, Type type)
        {
            var arguments = rawArguments.Split(" ");

            var command = _service.CreateCommand(arguments);

            Assert.NotNull(command);
            Assert.Equal(type, command.GetType());
            _loggerMock.Verify(logger => logger.Log
                (
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Never
            );
        }

        [Theory]
        [InlineData("-s")]
        [InlineData("--static")]
        public void ShouldReturnCommandWithFlags(string flag)
        {
            var arguments = new[] { "--generate", "--project", "Project", flag };

            var command = _service.CreateCommand(arguments.ToArray());
            var configureProjectCommand = command as GenerateProjectCommand;

            Assert.NotNull(configureProjectCommand);
            Assert.True(configureProjectCommand.Static);
            _loggerMock.Verify(logger => logger.Log
                (
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Never
            );
        }

        [Theory]
        [InlineData("-o")]
        [InlineData("--output")]
        public void ShouldReturnCommandWithExplicitParameters(string parameter)
        {
            var output = @"C:\Projetos\Tests";
            var arguments = new [] { 
                "--generate", "--project", "Project",
                parameter, output 
            };

            var command = _service.CreateCommand(arguments.ToArray());
            var configureProjectCommand = command as GenerateProjectCommand;

            Assert.NotNull(configureProjectCommand);
            Assert.Equal(output, configureProjectCommand.Output);
            _loggerMock.Verify(logger => logger.Log
                (
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Never
            );
        }

        [Fact]
        public void ShouldReturnCommandWithImplicitParameters()
        {
            var sourceClassName = "ContactRequest";
            var targetClassName = "ContactResponse";
            var comparatorName = "ObjectComparator";
            var arguments = new [] { 
                "--generate", "--compare", 
                sourceClassName, targetClassName, comparatorName 
            };

            var command = _service.CreateCommand(arguments.ToArray());
            var configureProjectCommand = command as GenerateCompareCommand;

            Assert.NotNull(configureProjectCommand);
            Assert.Equal(sourceClassName, configureProjectCommand.SourceClassName);
            Assert.Equal(targetClassName, configureProjectCommand.TargetClassName);
            Assert.Equal(comparatorName, configureProjectCommand.ComparatorName);
            _loggerMock.Verify(logger => logger.Log
                (
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Never
            );
        }
    }
}
