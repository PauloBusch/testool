using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Common;

namespace TesTool.Core.Interfaces.Services
{
    public interface IFixtureService
    {
        string GetName();
        string GetNamespace();
        string GetPathFile();
        Fixture GetModel(Class dbContextClass);
    }
}
