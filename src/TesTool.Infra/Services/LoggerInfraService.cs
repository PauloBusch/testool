using System;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class LoggerInfraService : ILoggerInfraService
    {
        public void LogError(string message, params object[] args) => Console.WriteLine(message, args);
        public void LogInformation(string message, params object[] args) => Console.WriteLine(message, args);
        public void LogWarning(string message, params object[] args) => Console.WriteLine(message, args);
    }
}
