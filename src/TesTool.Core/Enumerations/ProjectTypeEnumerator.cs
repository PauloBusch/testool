using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class ProjectTypeEnumerator : EnumeratorBase<ProjectTypeEnumerator, ProjectType>
    {
        public static readonly ProjectType WEB_API = new ("Web Api");
        public static readonly ProjectType INTEGRATION_TESTS = new ("Integration Tests");
    }
}
