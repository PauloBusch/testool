using System.Collections.Generic;

namespace TesTool.Core.Models.Metadata
{
    public class Dto : TypeBase
    {
        private readonly List<Property> _properties;

        public Dto(string @namespace) : base(@namespace, nameof(Dto))
        {
            _properties = new List<Property>();
        }

        public IEnumerable<Property> Properties => _properties.AsReadOnly();

        public void AddProperty(Property model) => _properties.Add(model);
    }
}
