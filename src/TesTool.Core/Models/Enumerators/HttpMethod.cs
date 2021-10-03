namespace TesTool.Core.Models.Enumerators
{
    public class HttpMethod
    {
        public HttpMethod(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
