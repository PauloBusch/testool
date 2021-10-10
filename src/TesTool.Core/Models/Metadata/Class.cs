using System.Collections.Generic;

namespace TesTool.Core.Models.Metadata
{
    public class Class : TypeBase
    {
        private readonly List<Property> _properties;
        private readonly List<Method> _methods;

        public Class(string name, string @namespace) 
            : base(nameof(Class), name, @namespace)
        {
            _properties = new List<Property>();
            _methods = new List<Method>();
        }

        public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();
        public IReadOnlyCollection<Method> Methods => _methods.AsReadOnly();

        public void AddProperty(Property property) => _properties.Add(property);
        public void AddMethod(Method method) => _methods.Add(method);
    }
}
