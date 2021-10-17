using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class TestClassEnumerator : EnumeratorBase<TestClassEnumerator, TestClass>
    {
        public static readonly TestClass FIXTURE = new("{PROJECT_NAME}Fixture");
        public static readonly TestClass REQUEST = new("Request");
        public static readonly TestClass TEST_BASE = new("TestBase");

        public static readonly TestClass MODEL_FACTORY = new("ModelFactory");
        public static readonly TestClass ENTITY_FACTORY = new("EntityFactory");
        public static readonly TestClass COMPARE_FACTORY = new("CompareFactory");
    }
}
