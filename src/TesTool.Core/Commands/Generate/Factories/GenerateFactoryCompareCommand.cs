using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("compare", HelpText = "Gerar fábrica de objetos de comparação.")]
    public class GenerateFactoryCompareCommand : GenerateFactoryBase
    {
        public GenerateFactoryCompareCommand(
            ISettingInfraService settingInfraService,
            IEnvironmentInfraService environmentInfraService,
            ITestScanInfraService testScanInfraService,
            IFileSystemInfraService fileSystemInfraService,
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            SettingEnumerator.COMPARATOR_FACTORY_NAME,
            TestClassEnumerator.COMPARATOR_FACTORY,
            settingInfraService, environmentInfraService,
            testScanInfraService, fileSystemInfraService,
            templateCodeInfraService
        ) { }

        protected override string BuildSourceCode(string factoryName)
        {
            throw new System.NotImplementedException();
        }
    }
}
