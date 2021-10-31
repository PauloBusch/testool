namespace TesTool.Core.Models.Configuration
{
    public class Setting
    {
        public string ConventionPathFile { get; set; }
        public string ProjectWebApiDirectory { get; set; }
        public string ProjectIntegrationTestDirectory { get; set; }
        public string DbContextName { get; set; }
        public string FixtureName { get; set; }
    }
}
