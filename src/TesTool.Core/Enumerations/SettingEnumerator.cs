using TesTool.Core.Models.Configuration;

namespace TesTool.Core.Enumerations
{
    public class SettingEnumerator : EnumeratorBase<SettingEnumerator, Setting>
    {
        public static readonly Setting CONVENTION_PATH_FILE = new("CONVENTION_PATH_FILE");
        public static readonly Setting PROJECT_DIRECTORY = new("PROJECT_DIRECTORY");

        public static readonly Setting MODEL_FACTORY_NAME = new("MODEL_FACTORY_NAME");
        public static readonly Setting ENTITY_FACTORY_NAME = new("ENTITY_FACTORY_NAME");
        public static readonly Setting COMPARATOR_FACTORY_NAME = new("COMPARATOR_FACTORY_NAME");
    }
}
