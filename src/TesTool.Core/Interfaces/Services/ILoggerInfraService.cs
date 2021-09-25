namespace TesTool.Core.Interfaces.Services
{
    public interface ILoggerInfraService
    {
        void LogError(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogInformation(string message, params object[] args);
    }
}
