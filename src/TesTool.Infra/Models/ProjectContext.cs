using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace TesTool.Infra.Models
{
    public class ProjectContext
    {
        public ProjectContext(ProjectId id)
        {
            Id = id;
            Classes = new List<ClassContext>();
        }

        public ProjectId Id { get; private set; }
        public List<ClassContext> Classes { get; private set; }
    }
}
