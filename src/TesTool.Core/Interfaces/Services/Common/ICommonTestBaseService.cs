using System.Collections.Generic;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Common;

namespace TesTool.Core.Interfaces.Services.Common
{
    public interface ICommonTestBaseService : ICommonServiceBase
    {
        TestBase GetTestBaseModel(Class dbContextClass, bool auth);
    }
}
