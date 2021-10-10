using System.Collections.Generic;
using System.Linq;

namespace TesTool.Core.Models.Metadata
{
    public class Dto : TypeBase
    {
        private readonly List<Property> _properties;

        public Dto(string fullNamespace) : base(fullNamespace, nameof(Dto))
        {
            _properties = new List<Property>();
        }

        public string Name => FullNamespace.Split(".").Last();
        public string Namespace => string.Join(".", FullNamespace.Split(".").Where(p => p != Name));
        public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();

        public void AddProperty(Property model) => _properties.Add(model);
    }
}
