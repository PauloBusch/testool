using System.Collections.Generic;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Models.Templates.Common
{
    public class Fixture
    {
        private List<string> _namespaces;

        public Fixture(
            Class dbContextClass, 
            string projectName, 
            string fixtureNamespace
        )
        {
            ProjectName = projectName;
            DbContext = dbContextClass.Name;
            FixtureNamespace = fixtureNamespace;
            _namespaces = new List<string>();
            AddNamespace(dbContextClass.Namespace);
        }

        public string DbContext { get; private set; }
        public string ProjectName { get; private set; }
        public string FixtureNamespace { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();

        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
