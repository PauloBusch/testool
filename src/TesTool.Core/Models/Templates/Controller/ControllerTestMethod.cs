using System.Collections.Generic;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Models.Templates.Controller
{
    public class ControllerTestMethod
    {
        private readonly List<string> _requiredNamespaces;

        public ControllerTestMethod(
            string name,
            HttpMethod method,
            ControllerTestMethodSectionArrage arrage,
            ControllerTestMethodSectionAct act,
            ControllerTestMethodSectionAssert assert
        )
        {
            Name = name;
            Method = method;
            Arrage = arrage;
            Act = act;
            Assert = assert;
            _requiredNamespaces = new List<string>();
        }

        public string Name { get; private set; }
        public HttpMethod Method { get; private set; }
        public IReadOnlyCollection<string> RequiredNamespaces => _requiredNamespaces.AsReadOnly();
        public ControllerTestMethodSectionArrage Arrage { get; private set; }
        public ControllerTestMethodSectionAct Act { get; private set; }
        public ControllerTestMethodSectionAssert Assert { get; private set; }
        public void AddRequiredNamespace(string @namespace)
            => _requiredNamespaces.Add(@namespace);
        public void AddRequiredNamespaces(IEnumerable<string> namespaces) 
            => _requiredNamespaces.AddRange(namespaces);
        public void Rename(string name) => Name = name;
    }
}
