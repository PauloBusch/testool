using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Comparator
{
    public class ModelCompare
    {
        private List<string> _namespaces;
        private List<ModelCompareProperty> _properties;
        private List<ModelCompareObject> _comparers;

        public ModelCompare(
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
            _properties = new List<ModelCompareProperty>();
            _comparers = new List<ModelCompareObject>();
        }

        public string ComparatorNamespace { get; private set; }
        public string ComparatorClassName { get; private set; }
        public string SourceClassName { get; private set; }
        public string TargetClassName { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<ModelCompareProperty> Properties => _properties.AsReadOnly();
        public IReadOnlyCollection<ModelCompareObject> Comparers => _comparers.AsReadOnly();

        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
        public void AddProperty(ModelCompareProperty property) => _properties.Add(property);
        public void AddComparer(ModelCompareObject comparer) => _comparers.Add(comparer);
    }
}
