using System;
using System.Diagnostics;
using System.IO;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class TesToolInfraService : ITesToolInfraService
    {
        public string GetVersion()
        {
            var testoolExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testool.exe");
            return FileVersionInfo.GetVersionInfo(testoolExe).FileVersion;
        }
    }
}
