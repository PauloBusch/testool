using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class InputSourceEnumerator : EnumeratorBase<InputSourceEnumerator, InputSource>
    {
        public static readonly InputSource BODY = new("Body");
        public static readonly InputSource FORM = new("Form");
        public static readonly InputSource ROUTE = new("Route");
        public static readonly InputSource QUERY = new("Query"); 
    }
}
