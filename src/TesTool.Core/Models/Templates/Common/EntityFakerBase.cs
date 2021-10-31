using System.Collections.Generic;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Models.Templates.Common
{
    public class EntityFakerBase
    {
        private List<string> _namespaces;

        public EntityFakerBase(
            string name, 
            string @namespace, 
            Class dbContextClass
        )
        {
            _namespaces = new List<string>();

            Name = name;
            Namespace = @namespace;
            DbContext = dbContextClass.Name;
            AddNamespace(dbContextClass.Namespace);
        }

        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public string DbContext { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();

        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
