using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Infra.Services;
using TesTool.IntegrationTests._Common;
using Xunit;

namespace TesTool.IntegrationTests.Services
{
    public class SettingsServiceTests : TestBase
    {
        private readonly ISettingInfraService _service;

        public SettingsServiceTests(TesToolFixture fixture) : base(fixture) { 
            _service = _services.GetRequiredService<ISettingInfraService>();    
        }

        [Theory]
        [InlineData("NUMBER", "123")]
        [InlineData("GUID", "1e7bb822-6032-410f-af85-d8e2ec05a439")]
        [InlineData("TEXT", "Lorem Ipsum é simplesmente uma simulação de texto...")]
        public async Task ShouldSaveAndReturnStringAsync(string key, string value)
        {
            var setting = SettingEnumerator.GetAll().Single(s => s.Key == key);
            await _service.SetStringAsync(setting, value);
            var result = await _service.GetStringAsync(setting);

            Assert.Equal(value, result);
        }
    }
}
