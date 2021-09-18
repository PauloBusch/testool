using Xunit;

namespace TesTool.IntegrationTests.Common
{
    public class TestBase : IClassFixture<TesToolFixture>
    {
        public TestBase(TesToolFixture fixture) { }
    }
}
