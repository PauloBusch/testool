namespace TesTool.Core.Models.Metadata
{
    public class Field : TypeBase
    {
        public Field(
            string @namespace, 
            string systemType
        ) : base(@namespace, nameof(Field)) { 
            SystemType = systemType;    
        }

        public string SystemType { get; private set; }
    }
}
