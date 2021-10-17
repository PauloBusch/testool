using System.Threading.Tasks;
using TesTool.Core.Models.Templates.Factory;

namespace TesTool.Core.Interfaces.Services.Factories
{
    public interface IFactoryCompareService
    {
        string GetNamespace();
        Task<string> GetFactoryNameAsync();
        ComparatorFactory GetModelFactory(string name);
    }
}
