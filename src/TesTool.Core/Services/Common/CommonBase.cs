using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Services.Common
{
    public abstract class CommonBase
    {
        protected readonly string _namespace;
        protected readonly HelpClass _helpClass;
        
        protected readonly ISolutionInfraService _solutionInfraService;
        protected readonly ITestScanInfraService _testScanInfraService;

        protected CommonBase(
            string @namespace, 
            HelpClass helpClass,
            ISolutionInfraService solutionInfraService,
            ITestScanInfraService testScanInfraService
        )
        {
            _namespace = @namespace;
            _helpClass = helpClass;
            _solutionInfraService = solutionInfraService;
            _testScanInfraService = testScanInfraService;
        }

        public string GetPathFile()
        {
            var directory = @$"{_testScanInfraService.GetDirectoryBase()}\{_namespace.Replace(".", @"\")}";
            return @$"{directory}\{_helpClass.Name}.cs";
        }

        public string GetNamespace()
        {
            return _solutionInfraService.GetTestNamespace(_namespace);
        }
    }
}
