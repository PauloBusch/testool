using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Interfaces.Services.Fakers
{
    public interface IFakeModelService
    {
        string GetNamespace();
        string GetDirectoryBase();
        string GetFakerName(string className);
        Task<ModelFaker> GetFakerModelAsync(Class model, bool @static);
    }
}
