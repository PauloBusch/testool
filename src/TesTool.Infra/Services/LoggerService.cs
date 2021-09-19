using System;
using TesTool.Core.Interfaces;

namespace TesTool.Infra.Services
{
    public class LoggerService<TCategoryName> : ILoggerService<TCategoryName>
    {
        public void LogError(string message, params object[] args) => Console.WriteLine(message, args);
        public void LogInformation(string message, params object[] args) => Console.WriteLine(message, args);
        public void LogWarning(string message, params object[] args) => Console.WriteLine(message, args);
    }
}
