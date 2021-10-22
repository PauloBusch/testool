using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Faker
{
    public abstract class FakerBase<TProperty> where TProperty : PropertyBase
    {
        private List<string> _namespaces;
        private List<TProperty> _properties;

        public FakerBase(string name, string fakerNamespace)
        {
            Name = name;
            FakerNamespace = fakerNamespace;
            _properties = new List<TProperty>();
            _namespaces = new List<string>();
        }

        public string Name { get; private set; }
        public string FakerNamespace { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<TProperty> Properties => _properties.AsReadOnly();

        public void AddProperty(TProperty property) => _properties.Add(property);
        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
