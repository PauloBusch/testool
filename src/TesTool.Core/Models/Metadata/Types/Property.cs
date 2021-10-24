namespace TesTool.Core.Models.Metadata
{
    public class Property
    {
        public Property(string name, TypeWrapper type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }
        public TypeWrapper Type { get; private set; }
    }
}
