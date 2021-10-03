using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class HttpMethodEnumerator : EnumeratorBase<HttpMethodEnumerator, HttpMethod>
    {
        public static readonly HttpMethod GET = new ("Get");
        public static readonly HttpMethod POST = new ("Post");
        public static readonly HttpMethod PUT = new ("Put");
        public static readonly HttpMethod PATCH = new ("Patch");
        public static readonly HttpMethod DELETE = new ("Delete");
    }
}
