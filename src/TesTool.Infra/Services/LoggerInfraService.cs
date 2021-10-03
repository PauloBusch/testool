using System;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class LoggerInfraService : ILoggerInfraService
    {
        public void LogError(string message) => Console.WriteLine(message);
        public void LogInformation(string message) => Console.WriteLine(message);
        public void LogWarning(string message) => Console.WriteLine(message);
    }
}
