using System;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class EnvironmentInfraService : IEnvironmentInfraService
    {
        public string GetWorkingDirectory() => Environment.CurrentDirectory;
    }
}
