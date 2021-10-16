using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class HelpClassEnumerator : EnumeratorBase<HelpClassEnumerator, HelpClass>
    {
        public static readonly HelpClass FIXTURE = new("Fixture", ".*fixture$");
        public static readonly HelpClass REQUEST = new("Request");
        public static readonly HelpClass TEST_BASE = new("TestBase");

        public static readonly HelpClass MODEL_FACTORY = new("ModelFactory", ".*modelfactory$");
        public static readonly HelpClass COMPARATOR_FACTORY = new("ComparatorFactory", ".*comparatorfactory$");
    }
}
