using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Common;

namespace TesTool.Core.Interfaces.Services.Common
{
    public interface ICommonEntityFakerBaseService : ICommonServiceBase 
    {
        EntityFakerBase GetEntityFakerBaseModel(Class dbContextClass);
    }
}
