using Xunit;

namespace TesTool.UnitTests._Common
{
    public class TestBase : IClassFixture<TesToolFixture>
    {
        public TestBase(TesToolFixture fixture) { }
    }
}
