using Microsoft.Extensions.DependencyInjection;

namespace TesTool.IntegrationTests.Common
{
    public class ServiceTestBase<TService> : TestBase
    {
        protected readonly TService _service;

        public ServiceTestBase(TesToolFixture fixture) : base(fixture)
        {
            _service = fixture.Services.GetRequiredService<TService>();
        }
    }
}
