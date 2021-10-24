using System.Collections.Generic;

namespace TesTool.Core.Models.Metadata
{
    public class Enum : TypeBase
    {
        public Enum(
            string name, 
            string @namespace,
            Dictionary<string, int> values
        ) : base(nameof(Enum), name, @namespace)
        {
            Values = values;
        }

        public Dictionary<string, int> Values { get; set; }
    }
}
