namespace TesTool.Core.Models.Metadata
{
    public class Array : TypeBase
    {
        public Array(
            string @namespace,
            TypeBase type
        ) : base(@namespace, nameof(Array)) { 
            Type = type;    
        }

        public TypeBase Type { get; private set; }
    }
}
