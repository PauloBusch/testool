using System.Collections.Generic;
using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Interfaces.Services
{
    public interface IWebApiDbContextInfraService
    {
        Task<bool> IsDbContextClassAsync(string className);
        Task<bool> IsDbSetClassAsync(string dbContext, string className);
        Task<IEnumerable<Class>> GetDbSetClassesAsync(string dbContext);
    }
}
