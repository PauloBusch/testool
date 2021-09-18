namespace TesTool.Core.Interfaces.Services
{
    public interface ICommandFactoryService
    {
        ICommand CreateCommand(string[] args);
    }
}
