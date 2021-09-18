using System;
using Xunit;

namespace TesTool.IntegrationTests.Common
{
    public class TestBase : IClassFixture<TesToolFixture>
    {
        protected readonly IServiceProvider Services;

        public TestBase(TesToolFixture fixture) { 
            Services = fixture.Services;    
        }
    }
}
