namespace TesTool.Core.Models.Metadata
{
    public class RequestCall
    {
        public RequestCall(
            string uri, 
            string method, 
            string returnType, 
            string bodyVariable, 
            string queryVariable
        )
        {
            Uri = uri;
            Method = method;
            ReturnType = returnType;
            BodyVariable = bodyVariable;
            QueryVariable = queryVariable;
        }

        public string Uri { get; private set; }
        public string Method { get; private set; }
        public string ReturnType { get; private set; }
        public string BodyVariable { get; private set; }
        public string QueryVariable { get; private set; }
    }
}
