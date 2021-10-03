using System.Collections.Generic;

namespace TesTool.Core.Models.Metadata
{
    public class Controller
    {
        private readonly List<Endpoint> _endpoints;

        public Controller(string name, string route)
        {
            Name = name;
            Route = route;
            _endpoints = new List<Endpoint>();
        }

        public string Name { get; private set; }
        public string Route { get; private set; }
        public IReadOnlyCollection<Endpoint> Endpoints => _endpoints.AsReadOnly();

        public void AddEndpoint(Endpoint endpoint) => _endpoints.Add(endpoint);
    }
}
