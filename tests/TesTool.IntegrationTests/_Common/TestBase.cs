using Xunit;

namespace TesTool.IntegrationTests._Common
{
    public class TestBase : IClassFixture<TesToolFixture>
    {
        public TestBase(TesToolFixture fixture) { }
    }
}
