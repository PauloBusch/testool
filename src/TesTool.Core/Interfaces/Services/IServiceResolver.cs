namespace TesTool.Core.Interfaces.Services
{
    public interface IServiceResolver
    {
        T ResolveService<T>() where T : class;
    }
}
