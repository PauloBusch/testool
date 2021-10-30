namespace TesTool.Core.Interfaces.Services
{
    public interface ISolutionInfraService
    {
        string GetSolutionName();
        string GetSolutionFilePath();
        string GetTestFixtureClassName();

        string GetTestName();
        string GetTestNamespace(string sufix = default);
    }
}
