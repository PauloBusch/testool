using System.Collections.Generic;

namespace TesTool.Core.Models.Metadata
{
    public class Enum : TypeBase
    {
        public Enum(
            string @namespace, 
            Dictionary<string, int> values
        ) : base(@namespace, nameof(Enum))
        {
            Values = values;
        }

        public Dictionary<string, int> Values { get; set; }
    }
}
