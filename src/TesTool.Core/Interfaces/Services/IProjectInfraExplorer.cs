using System;
using System.Collections.Generic;

namespace TesTool.Core.Interfaces.Services
{
    public interface IProjectInfraExplorer
    {
        bool IsTestProjectFile(string projectPathFile);
        string GetCurrentProject(Func<string, bool> filter = null);
        string GetProjectVersion(string projectPathFile);
        IEnumerable<string> GetProjectPackages(string projectPathFile);
    }
}
