using System.Threading.Tasks;

namespace TesTool.Core.Interfaces
{
    public interface ISettingsService
    {
        Task<string> GetStringAsync(string key);
        Task SetStringAsync(string key, string value);
    }
}
