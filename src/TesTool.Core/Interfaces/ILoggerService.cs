namespace TesTool.Core.Interfaces
{
    public interface ILoggerService<out TCategoryName>
    {
        void LogError(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogInformation(string message, params object[] args);
    }
}
