using System.Collections.Generic;
using System.Linq;

namespace TesTool.Core.Models.Metadata
{
    public class Enum : TypeBase
    {
        public Enum(
            string fullNamespace, 
            Dictionary<string, int> values
        ) : base(fullNamespace, nameof(Enum))
        {
            Values = values;
        }

        public string Name => FullNamespace.Split(".").Last();
        public string Namespace => string.Join(".", FullNamespace.Split(".").Where(p => p != Name));

        public Dictionary<string, int> Values { get; set; }
    }
}
