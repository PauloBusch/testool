using System.Threading.Tasks;
using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Interfaces.Services.Factories
{
    public interface IFactoryEntityService
    {
        string GetNamespace();
        Task<string> GetFactoryNameAsync();
        Task<EntityFakerFactory> GetEntityFactoryAsync(string name, string dbContext);
    }
}
