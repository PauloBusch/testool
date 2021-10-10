namespace TesTool.Core.Models.Metadata
{
    public class Array : TypeBase
    {
        public Array(
            string fullNamespace,
            TypeBase type
        ) : base(fullNamespace, nameof(Array)) { 
            Type = type;    
        }

        public TypeBase Type { get; private set; }
    }
}
