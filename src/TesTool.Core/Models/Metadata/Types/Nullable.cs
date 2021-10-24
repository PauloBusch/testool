namespace TesTool.Core.Models.Metadata
{
    public class Nullable : TypeWrapper
    {
        public Nullable(TypeWrapper type) : base(nameof(Nullable)) 
        { 
            Type = type;    
        }

        public TypeWrapper Type { get; private set; }
    }
}
