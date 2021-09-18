using Microsoft.Extensions.DependencyInjection;
using System;
using TesTool.Cli;

namespace TesTool.IntegrationTests
{
    public class TesToolFixture
    {
        public readonly IServiceProvider Services;

        public TesToolFixture()
        {
            Services = new ServiceCollection()
                .AddServices()
                .BuildServiceProvider();
        }
    }
}
