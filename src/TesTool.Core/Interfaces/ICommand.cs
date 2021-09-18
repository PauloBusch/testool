using System.Threading.Tasks;

namespace TesTool.Core.Interfaces
{
    public interface ICommand
    {
        Task ExecuteAsync();
    }
}
