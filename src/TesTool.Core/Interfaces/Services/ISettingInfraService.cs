using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface ISettingInfraService
    {
        Task<string> GetStringAsync(string key);
        Task SetStringAsync(string key, string value);
    }
}
