using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Interfaces.Services.Factories
{
    public interface IFactoryCompareService
    {
        string GetNamespace();
        string GetFactoryName();
        string GetDirectoryBase();
        ComparatorFactory GetModelFactory(string name);
    }
}
