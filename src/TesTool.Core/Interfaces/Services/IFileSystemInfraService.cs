using System.Threading.Tasks;

namespace TesTool.Core.Interfaces.Services
{
    public interface IFileSystemInfraService
    {
        Task<bool> FileExistAsync(string path);
        Task SaveFileAsync(string path, string content);
    }
}
