namespace TesTool.Core.Models.Metadata
{
    public class Property
    {
        public Property(string name, bool fromGeneric, TypeWrapper type)
        {
            Name = name;
            Type = type;
            FromGeneric = fromGeneric;
        }

        public string Name { get; private set; }
        public bool FromGeneric { get; private set; }
        public TypeWrapper Type { get; private set; }
    }
}
