using System.Collections.Generic;
using System.Linq;

namespace TesTool.Core.Models.Templates.Controller
{
    public class ControllerTestMethodSectionArrage
    {
        private readonly List<string> _entities;

        public ControllerTestMethodSectionArrage(
            IEnumerable<string> entities, 
            IEnumerable<string> models
        )
        {
            Models = models.ToList();
            _entities = entities.ToList();
        }

        public IReadOnlyCollection<string> Models { get; private set; }
        public IReadOnlyCollection<string> Entities => _entities.AsReadOnly();
        public bool IsEmpty => !Models.Concat(Entities).Any();

        public void RemoveEntity(string entity) => _entities.Remove(entity);
    }
}
