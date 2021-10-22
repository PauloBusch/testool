using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class TestClassEnumerator : EnumeratorBase<TestClassEnumerator, TestClass>
    {
        public static readonly TestClass FIXTURE = new("{PROJECT_NAME}Fixture");
        public static readonly TestClass REQUEST = new("Request");
        public static readonly TestClass TEST_BASE = new("TestBase");
        public static readonly TestClass ENTITY_FAKER_BASE = new("EntityFakerBase");

        // TODO: Rename classes
        public static readonly TestClass MODEL_FAKER_FACTORY = new("ModelFakerFactory1");
        public static readonly TestClass ENTITY_FAKER_FACTORY = new("EntityFakerFactory1");
        public static readonly TestClass COMPARE_FACTORY = new("CompareFactory1");
    }
}
