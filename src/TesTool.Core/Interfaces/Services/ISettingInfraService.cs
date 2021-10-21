using System.Threading.Tasks;
using TesTool.Core.Models.Configuration;

namespace TesTool.Core.Interfaces.Services
{
    public interface ISettingInfraService
    {
        Task<string> GetStringAsync(Setting setting);
        Task SetStringAsync(Setting setting, string value);
    }
}
