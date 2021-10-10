using System.Collections.Generic;
using System.Threading.Tasks;
using TesTool.Core.Models.Configuration;

namespace TesTool.Core.Interfaces.Services
{
    public interface IConventionInfraService
    {
        Task<IEnumerable<Convention>> GetConfiguredConventionsAsync();
    }
}
