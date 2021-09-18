using Xunit;

namespace TesTool.UnitTests.Common
{
    public class TestBase : IClassFixture<TesToolFixture>
    {
        public TestBase(TesToolFixture fixture) { }
    }
}
