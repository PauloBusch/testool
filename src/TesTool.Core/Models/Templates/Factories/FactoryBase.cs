using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Factories
{
    public abstract class FactoryBase<TMethod>
    {
        private List<string> _namespaces;
        private readonly List<TMethod> _methods;

        public FactoryBase(
            string name,
            string factoryNamespace
        )
        {
            Name = name;
            FactoryNamespace = factoryNamespace;
            _methods = new List<TMethod>();
            _namespaces = new List<string>();
        }

        public string Name { get; private set; }
        public string FactoryNamespace { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<TMethod> Methods => _methods.AsReadOnly();

        public void AddMethod(TMethod method) => _methods.Add(method);
        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
