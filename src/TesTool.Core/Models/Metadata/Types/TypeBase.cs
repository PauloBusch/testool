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
    }
}
