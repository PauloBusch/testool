using System.Collections.Generic;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Models.Metadata
{
    public class Endpoint
    {
        private readonly List<Input> _inputs;

        public Endpoint(string route, string name, bool authorize, HttpMethod method)
        {
            Name = name;
            Route = route;
            Method = method;
            Authorize = authorize;
            _inputs = new List<Input>();
        }

        public string Name { get; private set; }
        public string Route { get; private set; }
        public bool Authorize { get; private set; }
        public HttpMethod Method { get; private set; }
        public IReadOnlyCollection<Input> Inputs => _inputs.AsReadOnly();
        public TypeWrapper Output { get; set; }

        public void AddInput(Input input) => _inputs.Add(input);
    }
}
