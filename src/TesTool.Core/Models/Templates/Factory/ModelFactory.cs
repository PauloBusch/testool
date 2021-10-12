using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Factory
{
    public class ModelFactory
    {
        private List<string> _namespaces;
        private readonly List<ModelFactoryMethod> _methods;

        public ModelFactory(string name, string factoryNamespace)
        {
            Name = name;
            FactoryNamespace = factoryNamespace;
            _methods = new List<ModelFactoryMethod>();
            _namespaces = new List<string>();
        }

        public string Name { get; private set; }
        public string FactoryNamespace { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<ModelFactoryMethod> Methods => _methods.AsReadOnly();

        public void AddMethod(ModelFactoryMethod method) => _methods.Add(method);
        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
