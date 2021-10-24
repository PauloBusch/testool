namespace TesTool.Core.Models.Enumerators
{
    public class TestMethod
    {
        public TestMethod(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
