using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Comparator
{
    public class CompareDynamic
    {
        private List<string> _namespaces;

        public CompareDynamic(
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
        }

        public string ComparatorNamespace { get; private set; }
        public string ComparatorClassName { get; private set; }
        public string SourceClassName { get; private set; }
        public string TargetClassName { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();

        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
