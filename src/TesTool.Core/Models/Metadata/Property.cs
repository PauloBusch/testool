namespace TesTool.Core.Models.Metadata
{
    public class Property
    {
        public Property(string name, TypeBase type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }
        public TypeBase Type { get; private set; }
    }
}
