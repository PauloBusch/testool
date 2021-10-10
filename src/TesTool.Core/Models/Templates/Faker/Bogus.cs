using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Faker
{
    public class Bogus
    {
        private List<string> _namespaces;
        private List<BogusProperty> _properties;

        public Bogus(string name, string fakerNamespace)
        {
            Name = name;
            FakerNamespace = fakerNamespace;
            _properties = new List<BogusProperty>();
            _namespaces = new List<string>();
        }

        public string Name { get; private set; }
        public string FakerNamespace { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<BogusProperty> Properties => _properties.AsReadOnly();

        public void AddProperty(BogusProperty property) => _properties.Add(property);
        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
