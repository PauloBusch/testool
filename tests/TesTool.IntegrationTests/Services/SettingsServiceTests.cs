using System.Threading.Tasks;
using TesTool.Infra.Services;
using TesTool.IntegrationTests._Common;
using Xunit;

namespace TesTool.IntegrationTests.Services
{
    public class SettingsServiceTests : TestBase
    {
        public SettingsServiceTests(TesToolFixture fixture) : base(fixture) { }

        [Theory]
        [InlineData("NUMBER", "123")]
        [InlineData("GUID", "1e7bb822-6032-410f-af85-d8e2ec05a439")]
        [InlineData("TEXT", "Lorem Ipsum é simplesmente uma simulação de texto...")]
        public async Task ShouldSaveAndReturnStringAsync(string key, string value)
        {
            var service = new SettingsService();

            await service.SetStringAsync(key, value);
            var result = await service.GetStringAsync(key);

            Assert.Equal(value, result);
        }
    }
}
