namespace TesTool.Core.Interfaces.Services
{
    public interface ILoggerInfraService
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogInformation(string message);
    }
}
