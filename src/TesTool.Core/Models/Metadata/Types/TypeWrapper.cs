namespace TesTool.Core.Models.Metadata
{
    public abstract class TypeWrapper
    {
        protected TypeWrapper(string wrapper)
        {
            Wrapper = wrapper;
        }

        public string Wrapper { get; protected set; }
    }
}
