namespace TesTool.Core.Models.Metadata
{
    public class Field : TypeBase
    {
        public Field(
            string fullNamespace, 
            string systemType
        ) : base(fullNamespace, nameof(Field)) { 
            SystemType = systemType;    
        }

        public string SystemType { get; private set; }
    }
}
