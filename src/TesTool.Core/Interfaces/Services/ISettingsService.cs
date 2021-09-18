using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface ISettingsService
    {
        Task<string> GetStringAsync(string key);
        Task SetStringAsync(string key, string value);
    }
}
