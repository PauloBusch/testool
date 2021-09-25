using TesTool.Core.Models;

namespace TesTool.Core.Enumerations
{
    public class SettingEnumerator : EnumeratorBase<SettingEnumerator, Setting>
    {
        public static readonly Setting CONVENTION_PATH_FILE = new Setting("CONVENTION_PATH_FILE");
        public static readonly Setting PROJECT_DIRECTORY = new Setting("PROJECT_DIRECTORY");
    }
}
