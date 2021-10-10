namespace TesTool.Core.Models.Metadata
{
    public abstract class TypeBase
    {
        protected TypeBase(string fullNamespace, string wrappper)
        {
            FullNamespace = fullNamespace;
            Wrapper = wrappper;
        }

        public string Wrapper { get; protected set; }
        public string FullNamespace { get; protected set; }
    }
}
