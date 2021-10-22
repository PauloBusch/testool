using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class EnvironmentInfraService : IEnvironmentInfraService
    {
        // TODO: Uncomment code
        //public string GetWorkingDirectory() => Environment.CurrentDirectory;
        //public string GetWorkingDirectory() => @"C:\Projetos\Cip\Tests\Loan.IntegrationTests\Fakers";
        public string GetWorkingDirectory() => @"C:\Projetos\Fcb.Api\tests\Fcb.Api.IntegrationTests\Fakers\Entities";
    }
}
