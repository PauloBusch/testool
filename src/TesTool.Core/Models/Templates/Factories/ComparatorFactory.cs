using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Factory
{
    public class ComparatorFactory
    {
        private List<string> _namespaces;
        private readonly List<ComparatorFactoryMethod> _methods;

        public ComparatorFactory(
            string name, 
            string factoryNamespace,
            bool @static
        )
        {
            Name = name;
            Static = @static;
            FactoryNamespace = factoryNamespace;
            _methods = new List<ComparatorFactoryMethod>();
            _namespaces = new List<string>();
        }

        public bool Static { get; private set; }
        public string Name { get; private set; }
        public string FactoryNamespace { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<ComparatorFactoryMethod> Methods => _methods.AsReadOnly();

        public void AddMethod(ComparatorFactoryMethod method) => _methods.Add(method);
        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
