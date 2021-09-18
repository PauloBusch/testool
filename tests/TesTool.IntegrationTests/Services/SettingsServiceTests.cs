using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;
using TesTool.IntegrationTests.Common;
using Xunit;

namespace TesTool.IntegrationTests.Services
{
    public class SettingsServiceTests : TestBase
    {
        private readonly ISettingsService _service;
        public SettingsServiceTests(TesToolFixture fixture) : base(fixture) { 
            _service = Services.GetRequiredService<ISettingsService>();
        }

        [Theory]
        [InlineData("NUMBER", "123")]
        [InlineData("GUID", "1e7bb822-6032-410f-af85-d8e2ec05a439")]
        [InlineData("TEXT", "Lorem Ipsum é simplesmente uma simulação de texto...")]
        public async Task ShouldSaveAndReturnStringAsync(string key, string value)
        {
            await _service.SetStringAsync(key, value);
            var result = await _service.GetStringAsync(key);

            Assert.Equal(value, result);
        }
    }
}
