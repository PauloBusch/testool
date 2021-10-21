using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Faker
{
    public class ModelFaker
    {
        private List<string> _namespaces;
        private List<ModelProperty> _properties;

        public ModelFaker(string name, string fakerNamespace)
        {
            Name = name;
            FakerNamespace = fakerNamespace;
            _properties = new List<ModelProperty>();
            _namespaces = new List<string>();
        }

        public string Name { get; private set; }
        public string FakerNamespace { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<ModelProperty> Properties => _properties.AsReadOnly();

        public void AddProperty(ModelProperty property) => _properties.Add(property);
        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
