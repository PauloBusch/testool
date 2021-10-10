using System.Collections.Generic;

namespace TesTool.Core.Models.Metadata
{
    public class Dto : TypeBase
    {
        private readonly List<Property> _properties;

        public Dto(string name, string @namespace) 
            : base(nameof(Dto), name, @namespace)
        {
            _properties = new List<Property>();
        }

        public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();

        public void AddProperty(Property model) => _properties.Add(model);
    }
}
