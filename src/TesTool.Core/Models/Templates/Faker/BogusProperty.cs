namespace TesTool.Core.Models.Templates.Faker
{
    public class BogusProperty
    {
        public BogusProperty(string name, string expression, bool @unsafe)
        {
            Name = name;
            Unsafe = @unsafe;
            Expression = expression;
        }

        public bool Unsafe { get; set; }
        public string Name { get; private set; }
        public string Expression { get; private set; }
    }
}
