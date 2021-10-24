using System.Threading.Tasks;
using TesTool.Core.Models.Templates.Factories;

namespace TesTool.Core.Interfaces.Services.Factories
{
    public interface IFactoryEntityService
    {
        string GetNamespace();
        string GetFactoryName();
        string GetDirectoryBase();
        Task<EntityFakerFactory> GetEntityFactoryAsync(string name, string dbContext);
    }
}
