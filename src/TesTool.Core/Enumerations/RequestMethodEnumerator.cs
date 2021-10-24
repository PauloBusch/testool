using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class RequestMethodEnumerator : EnumeratorBase<RequestMethodEnumerator, RequestMethod>
    {
        public static readonly RequestMethod GET = new("GetAsync");
        public static readonly RequestMethod POST = new("PostAsync");
        public static readonly RequestMethod PUT = new("PutAsync");
        public static readonly RequestMethod PATCH = new("PatchAsync");
        public static readonly RequestMethod DELETE = new("DeleteAsync");
    }
}
