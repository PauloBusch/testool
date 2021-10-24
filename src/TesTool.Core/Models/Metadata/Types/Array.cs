namespace TesTool.Core.Models.Metadata
{
    public class Array : TypeBase
    {
        public Array(string name, string @namespace, TypeWrapper type) 
            : base(nameof(Array), name, @namespace) 
        { 
            Type = type;    
        }

        public TypeWrapper Type { get; private set; }
    }
}
