using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Models.Metadata
{
    public class Input : Property
    {
        public Input(
            string name, 
            TypeWrapper type, 
            InputSource source
        ) : base(name, default, type)
        {
            Source = source;
        }

        public InputSource Source { get; private set; }
    }
}
