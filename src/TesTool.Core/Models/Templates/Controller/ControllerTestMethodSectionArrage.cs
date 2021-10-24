using System.Collections.Generic;
using System.Linq;

namespace TesTool.Core.Models.Templates.Controller
{
    public class ControllerTestMethodSectionArrage
    {
        public ControllerTestMethodSectionArrage(
            IEnumerable<string> entities, 
            IEnumerable<string> models
        )
        {
            Models = models.ToList();
            Entities = entities.ToList();
        }

        public IReadOnlyCollection<string> Models { get; private set; }
        public IReadOnlyCollection<string> Entities { get; private set; }
    }
}
