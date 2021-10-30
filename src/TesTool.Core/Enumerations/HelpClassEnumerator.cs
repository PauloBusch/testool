using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class HelpClassEnumerator : EnumeratorBase<HelpClassEnumerator, HelpClass>
    {
        public static readonly HelpClass FIXTURE = new("{PROJECT_NAME}Fixture");
        public static readonly HelpClass REQUEST = new("Request");
        public static readonly HelpClass TEST_BASE = new("TestBase");
        public static readonly HelpClass ENTITY_FAKER_BASE = new("EntityFakerBase");
        public static readonly HelpClass PROJECT_EXPLORER = new("ProjectExplorer");
        public static readonly HelpClass CONFIGURATION_LOADER = new("ConfigurationLoader");

        public static readonly HelpClass MODEL_FAKER_FACTORY = new("ModelFakerFactory");
        public static readonly HelpClass ENTITY_FAKER_FACTORY = new("EntityFakerFactory");
        public static readonly HelpClass COMPARE_FACTORY = new("CompareFactory");
    }
}
