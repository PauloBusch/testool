using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class FileSystemInfraService : IFileSystemInfraService
    {
        public async Task<bool> FileExistAsync(string path)
        {
            return await Task.FromResult(File.Exists(path));
        }

        public async Task SaveFileAsync(string path, string content)
        {
            await File.WriteAllTextAsync(path, content);
        }
    }
}
