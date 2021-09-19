namespace TesTool.Core.Interfaces
{
    public interface ILoggerService
    {
        void LogError(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogInformation(string message, params object[] args);
    }
}
