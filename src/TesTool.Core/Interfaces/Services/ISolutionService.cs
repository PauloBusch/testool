namespace TesTool.Core.Interfaces.Services
{
    public interface ISolutionService
    {
        string GetSolutionName();
        string GetTestFixtureClassName();
        string GetTestNamespace(string sufix = default);
    }
}
