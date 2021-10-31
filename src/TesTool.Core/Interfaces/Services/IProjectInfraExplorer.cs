using System;
using System.Collections.Generic;

namespace TesTool.Core.Interfaces.Services
{
    public interface IProjectInfraExplorer
    {
        bool IsTestProjectFile(string projectPathFile);
        string GetCurrentProject(Func<string, bool> filter = null);
        IEnumerable<string> GetProjectPackages(string projectPathFile);
    }
}
