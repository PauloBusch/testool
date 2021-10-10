using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Exceptions
{
    public class ProjectNotFoundException : TesToolExceptionBase
    {
        public ProjectNotFoundException(ProjectType type) : base($"Projeto {type.Name} não encontrado.") { }
    }
}
