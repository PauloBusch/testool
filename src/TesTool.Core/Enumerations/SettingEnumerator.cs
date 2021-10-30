using TesTool.Core.Models.Configuration;

namespace TesTool.Core.Enumerations
{
    public class SettingEnumerator : EnumeratorBase<SettingEnumerator, Setting>
    {
        public static readonly Setting CONVENTION_PATH_FILE = new("CONVENTION_PATH_FILE");
        public static readonly Setting PROJECT_WEB_API_DIRECTORY = new("PROJECT_WEB_API_DIRECTORY");
        public static readonly Setting PROJECT_INTEGRATION_TEST_DIRECTORY = new("PROJECT_INTEGRATION_TEST_DIRECTORY");

        public static readonly Setting FIXTURE_NAME = new("FIXTURE_NAME");
        public static readonly Setting DB_CONTEXT_NAME = new("DB_CONTEXT_NAME");
    }
}
