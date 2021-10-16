using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Comparator
{
    public class CompareStatic
    {
        private List<string> _namespaces;
        private List<CompareProperty> _properties;
        private List<CompareObject> _comparers;

        public CompareStatic(
            string comparatorNamespace, 
            string comparatorClassName, 
            string sourceClassName, 
            string targetClassName
        )
        {
            ComparatorNamespace = comparatorNamespace;
            ComparatorClassName = comparatorClassName;
            SourceClassName = sourceClassName;
            TargetClassName = targetClassName;
            _namespaces = new List<string>();
            _properties = new List<CompareProperty>();
            _comparers = new List<CompareObject>();
        }

        public string ComparatorNamespace { get; private set; }
        public string ComparatorClassName { get; private set; }
        public string SourceClassName { get; private set; }
        public string TargetClassName { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<CompareProperty> Properties => _properties.AsReadOnly();
        public IReadOnlyCollection<CompareObject> Comparers => _comparers.AsReadOnly();

        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
        public void AddProperty(CompareProperty property) => _properties.Add(property);
        public void AddComparer(CompareObject comparer) => _comparers.Add(comparer);
    }
}
