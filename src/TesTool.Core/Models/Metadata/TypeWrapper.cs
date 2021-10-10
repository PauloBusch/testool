namespace TesTool.Core.Models.Metadata
{
    public abstract class TypeWrapper
    {
        protected TypeWrapper(string wrappper)
        {
            Wrapper = wrappper;
        }

        public string Wrapper { get; protected set; }
    }
}
