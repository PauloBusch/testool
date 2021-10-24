using System.Collections.Generic;

namespace TesTool.Core.Models.Metadata
{
    public class Class : TypeBase
    {
        private List<string> _namespaces;
        private readonly List<Method> _methods;
        private readonly List<Property> _properties;
        private readonly List<TypeWrapper> _generics;

        public Class(string name, string @namespace) 
            : base(nameof(Class), name, @namespace)
        {
            _properties = new List<Property>();
            _namespaces = new List<string>();
            _generics = new List<TypeWrapper>();
            _methods = new List<Method>();
        }

        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();
        public IReadOnlyCollection<TypeWrapper> Generics => _generics.AsReadOnly();
        public IReadOnlyCollection<Method> Methods => _methods.AsReadOnly();

        public void AddProperty(Property property) => _properties.Add(property);
        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
        public void AddGeneric(TypeWrapper generic) => _generics.Add(generic);
        public void AddMethod(Method method) => _methods.Add(method);
    }
}
