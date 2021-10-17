using System.Threading.Tasks;
using TesTool.Core.Models.Templates.Factory;

namespace TesTool.Core.Interfaces.Services.Factories
{
    public interface IFactoryModelService
    {
        string GetNamespace();
        Task<string> GetFactoryNameAsync();
        ModelFactory GetModelFactory(string name);
    }
}
