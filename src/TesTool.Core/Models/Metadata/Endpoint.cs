using System.Collections.Generic;
using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Models.Metadata
{
    public class Endpoint
    {
        private readonly List<Input> _inputs;

        public Endpoint(string route, Method method)
        {
            Route = route;
            Method = method;
            _inputs = new List<Input>();
        }

        public string Route { get; private set; }
        public Method Method { get; private set; }
        public IReadOnlyCollection<Input> Inputs => _inputs.AsReadOnly();
        public TypeBase Output { get; set; }

        public void AddInput(Input input) => _inputs.Add(input);
    }
}
