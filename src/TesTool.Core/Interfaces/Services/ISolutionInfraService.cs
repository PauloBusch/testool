namespace TesTool.Core.Interfaces.Services
{
    public interface ISolutionInfraService
    {
        string GetSolutionName();
        string GetSolutionFilePath();

        string GetTestProjectName();
        string GetTestProjectNamespace(string sufix = default);
    }
}
