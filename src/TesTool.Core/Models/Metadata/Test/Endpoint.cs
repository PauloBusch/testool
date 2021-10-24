using System.Collections.Generic;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Models.Metadata
{
    public class Endpoint
    {
        private readonly List<Input> _inputs;

        public Endpoint(string route, bool authorize, HttpMethod method)
        {
            Route = route;
            Method = method;
            Authorize = authorize;
            _inputs = new List<Input>();
        }

        public string Route { get; private set; }
        public bool Authorize { get; private set; }
        public HttpMethod Method { get; private set; }
        public IReadOnlyCollection<Input> Inputs => _inputs.AsReadOnly();
        public TypeWrapper Output { get; set; }

        public void AddInput(Input input) => _inputs.Add(input);
    }
}
