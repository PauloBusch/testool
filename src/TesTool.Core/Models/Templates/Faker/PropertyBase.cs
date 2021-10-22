namespace TesTool.Core.Models.Templates.Faker
{
    public abstract class PropertyBase
    {
        public PropertyBase(string name, string expression, bool @unsafe)
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
