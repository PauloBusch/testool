using System.Collections.Generic;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Models.Templates.Controller
{
    public class ControllerTest
    {
        private readonly List<string> _namespaces;
        private readonly List<ControllerTestMethod> _methods;

        public ControllerTest(
            string name,
            string baseRoute,
            string @namespace,
            Class fixture,
            Class testBase
        )
        {
            Name = name;
            BaseRoute = baseRoute;
            Namespace = @namespace;
            FixtureName = fixture.Name;
            _methods = new List<ControllerTestMethod>();
            _namespaces = new List<string>();

            AddNamespace(fixture.Namespace);
            AddNamespace(testBase.Namespace);
        }

        public string Name { get; private set; }
        public string BaseRoute { get; private set; }
        public string Namespace { get; private set; }
        public string FixtureName { get; private set; }
        public IReadOnlyCollection<string> Namespaces => _namespaces.AsReadOnly();
        public IReadOnlyCollection<ControllerTestMethod> Methods => _methods.AsReadOnly();

        public void AddMethod(ControllerTestMethod method) => _methods.Add(method);
        public void AddNamespace(string @namespace) => _namespaces.Add(@namespace);
    }
}
