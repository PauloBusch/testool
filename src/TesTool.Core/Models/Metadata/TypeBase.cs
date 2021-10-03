namespace TesTool.Core.Models.Metadata
{
    public abstract class TypeBase
    {
        protected TypeBase(string @namespace, string wrappper)
        {
            Wrapper = wrappper;
            Namespace = @namespace;
        }

        public string Wrapper { get; protected set; }
        public string Namespace { get; protected set; }
    }
}
