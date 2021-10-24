namespace TesTool.Core.Models.Metadata
{
    public class Field : TypeBase
    {
        public Field(
            string name, 
            string @namespace
        ) : base(nameof(Field), name, @namespace) 
        { 
            SystemType = $"{@namespace}.{name}";    
        }

        public string SystemType { get; private set; }
    }
}
