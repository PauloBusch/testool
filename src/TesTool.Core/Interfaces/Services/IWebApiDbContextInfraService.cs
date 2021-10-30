using System.Collections.Generic;
using System.Threading.Tasks;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;

namespace TesTool.Core.Interfaces.Services
{
    public interface IWebApiDbContextInfraService
    {
        Task<bool> IsDbContextClassAsync(string className);
        Task<bool> IsDbSetClassAsync(string dbContext, string className);
        Task<IEnumerable<DbSet>> GetDbSetsAsync(string dbContext);
        Task<IEnumerable<Class>> GetDbContextClassesAsync();
    }
}
