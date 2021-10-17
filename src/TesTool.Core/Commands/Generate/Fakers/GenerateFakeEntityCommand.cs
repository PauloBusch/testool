using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Commands.Generate.Fakers
{
    [Command("entity", HelpText = "Gerar código de fabricação de entidade de banco de dados.")]
    public class GenerateFakeEntityCommand : GenerateFakeBase
    {
        public GenerateFakeEntityCommand(
            IEnvironmentInfraService environmentInfraService, 
            IFileSystemInfraService fileSystemInfraService, 
            IWebApiScanInfraService webApiScanInfraService, 
            ITestScanInfraService testScanInfraService, 
            ITemplateCodeInfraService templateCodeInfraService
        ) : base(
            environmentInfraService, 
            fileSystemInfraService, webApiScanInfraService, 
            testScanInfraService, templateCodeInfraService
        )
        { }

        protected override Task<string> GetFactoryNameAsync()
        {
            throw new System.NotImplementedException();
        }

        protected override Task AppendFactoryMethodAsync(Class @class, string fakerName, string factoryName)
        {
            throw new System.NotImplementedException();
        }

        protected override Task<string> BuildSourceCodeAsync(Class @class, string fakerName)
        {
            throw new System.NotImplementedException();
        }
    }
}
