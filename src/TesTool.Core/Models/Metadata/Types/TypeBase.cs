namespace TesTool.Core.Models.Metadata
{
    public abstract class TypeBase : TypeWrapper
    {
        protected TypeBase(
            string wrappper,
            string name, 
            string @namespace 
        ) : base(wrappper) 
        {
            Name = name;
            Namespace = @namespace;
        }

        public string Name { get; private set; }
        public string Namespace { get; private set; }

        public override bool Equals(object @object)
        {
            if (@object is not TypeBase compareTo) return false;
            if (ReferenceEquals(this, compareTo)) return true;
            return Wrapper == compareTo.Wrapper && 
                Name == compareTo.Name && 
                Namespace == compareTo.Namespace;
        }

        public static bool operator ==(TypeBase source, TypeBase target)
        {
            if (source is null && target is null) return true;
            if (source is null || target is null) return false;
            return source.Equals(target);
        }

        public static bool operator !=(TypeBase source, TypeBase target) => !(source == target);

        public override int GetHashCode() => GetType().GetHashCode();
    }
}
