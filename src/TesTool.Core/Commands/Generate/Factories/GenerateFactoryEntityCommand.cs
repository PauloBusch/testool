using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate.Factory
{
    [Command("entity", HelpText = "Gerar fábrica de entidades de banco de dados.")]
    public class GenerateFactoryEntityCommand : GenerateFactoryBase
    {
        public GenerateFactoryEntityCommand(
            ISettingInfraService settingInfraService, 
            IEnvironmentInfraService environmentInfraService, 
            ITestScanInfraService testScanInfraService, 
            IFileSystemInfraService fileSystemInfraService, 
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            SettingEnumerator.ENTITY_FACTORY_NAME,
            TestClassEnumerator.ENTITY_FACTORY,
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
