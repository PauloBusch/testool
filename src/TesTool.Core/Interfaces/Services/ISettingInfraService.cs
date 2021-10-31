namespace TesTool.Core.Interfaces.Services
{
    public interface ISettingInfraService
    {
        string ConventionPathFile { get; set; }
        string ProjectWebApiDirectory { get; set; }
        string ProjectIntegrationTestDirectory { get; set; }
        string DbContextName { get; set; }
        string FixtureName { get; set; }

        void CreateTemporarySetting();
    }
}
