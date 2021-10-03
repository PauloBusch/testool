using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class MethodEnumerator : EnumeratorBase<MethodEnumerator, Method>
    {
        public static readonly Method GET = new ("Get");
        public static readonly Method POST = new ("Post");
        public static readonly Method PUT = new ("Put");
        public static readonly Method PATCH = new ("Patch");
        public static readonly Method DELETE = new ("Delete");
    }
}
