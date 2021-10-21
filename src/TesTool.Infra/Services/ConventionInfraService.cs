using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Configuration;

namespace TesTool.Infra.Services
{
    public class ConventionInfraService : IConventionInfraService
    {
        private readonly ISettingInfraService _settingInfraService;

        public ConventionInfraService(ISettingInfraService settingInfraService)
        {
            _settingInfraService = settingInfraService;
        }

        public async Task<IEnumerable<Convention>> GetConfiguredConventionsAsync()
        {
            var defaultConventions = GetDefaultConventions();
            var conventionPathFile = await _settingInfraService.GetStringAsync(SettingEnumerator.CONVENTION_PATH_FILE);
            if (string.IsNullOrWhiteSpace(conventionPathFile) || !File.Exists(conventionPathFile)) return defaultConventions;

            var conventionJson = File.ReadAllText(conventionPathFile);
            var conventions = JsonSerializer.Deserialize<List<Convention>>(conventionJson);
            conventions.AddRange(defaultConventions);
            return conventions;
        }

        private IEnumerable<Convention> GetDefaultConventions()
        {
            return new List<Convention> {
                new Convention(
                    BogusMethodEnumerator.RANDOM_GUID.Expression,
                    typeMatch: $"^System.Guid$"
                ),
                new Convention(
                    BogusMethodEnumerator.RANDOM_BOOL.Expression,
                    typeMatch: "^System.Boolean$"
                ),
                new Convention(
                    BogusMethodEnumerator.RANDOM_INT.Expression,
                    typeMatch: "System.Int32$"
                ),
                new Convention(
                    BogusMethodEnumerator.RANDOM_DECIMAL.Expression,
                    typeMatch: $"^System.Decimal$"
                ),
                new Convention(
                    BogusMethodEnumerator.LOREM_WORD.Expression,
                    typeMatch: $"^System.String$"
                ),

                new Convention(
                    BogusMethodEnumerator.LOREM_PARAGRAPH.Expression,
                    propertyMatch: "description|justification|text",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.ADDRESS_CITY.Expression,
                    propertyMatch: "city",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.ADDRESS_STATE.Expression,
                    propertyMatch: "state",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.ADDRESS_COUNTRY.Expression,
                    propertyMatch: "country|nationality",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.ADDRESS_ZIP_CODE.Expression,
                    propertyMatch: "zipcode",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.PERSON_FULL_NAME.Expression,
                    propertyMatch: "name|full_?name",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.PERSON_FIRST_NAME.Expression,
                    propertyMatch: "first_?name",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.PERSON_LAST_NAME.Expression,
                    propertyMatch: "last_?name",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.PERSON_USER_NAME.Expression,
                    propertyMatch: "user_?name|login|user",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.INTERNET_PASSWORD.Expression,
                    propertyMatch: "password",
                    typeMatch: $"^System.String$"
                ),
                new Convention(
                    BogusMethodEnumerator.PERSON_EMAIL.Expression,
                    propertyMatch: "email",
                    typeMatch: $"^System.String$"
                )
            };
        }
    }
}
