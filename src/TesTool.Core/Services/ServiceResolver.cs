using System;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Services
{
    public class ServiceResolver : IServiceResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T ResolveService<T>() where T : class
        {
            var type = typeof(T);
            var service = _serviceProvider.GetService(type) as T;
            if (service is null) throw new InvalidOperationException($"Unable resolve service: {type.Name}.");
            return service;
        }
    }
}
