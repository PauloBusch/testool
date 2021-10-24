namespace TesTool.Core.Models.Enumerators
{
    public class RequestMethod
    {
        public RequestMethod(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
