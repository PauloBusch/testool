using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Interfaces.Services.Factories
{
    public interface IFactoryModelService
    {
        string GetNamespace();
        string GetFactoryName();
        string GetDirectoryBase();
        ModelFakerFactory GetModelFactory(string name);
    }
}
