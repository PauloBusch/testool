using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Models.Metadata
{
    public class Input : Property
    {
        public Input(
            string name, 
            TypeBase type, 
            InputSource source
        ) : base(name, type)
        {
            Source = source;
        }

        public InputSource Source { get; private set; }
    }
}
