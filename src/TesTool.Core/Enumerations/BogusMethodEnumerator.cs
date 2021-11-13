using TesTool.Core.Models.Enumerators;

namespace TesTool.Core.Enumerations
{
    public class BogusMethodEnumerator : EnumeratorBase<BogusMethodEnumerator, BogusMethod>
    {
        public static readonly BogusMethod LOREM_WORD = new ("f => f.Lorem.Word()");
        public static readonly BogusMethod LOREM_TEXT = new ("f => f.Lorem.Text()");
        public static readonly BogusMethod LOREM_PARAGRAPH = new ("f => f.Lorem.Paragraph()");

        public static readonly BogusMethod ADDRESS_CITY = new ("f => f.Address.City()");
        public static readonly BogusMethod ADDRESS_STATE = new ("f => f.Address.State()");
        public static readonly BogusMethod ADDRESS_COUNTRY = new ("f => f.Address.County()");
        public static readonly BogusMethod ADDRESS_ZIP_CODE = new ("f => f.Address.ZipCode()");

        public static readonly BogusMethod RANDOM_GUID = new ("f => f.Random.Guid()");
        public static readonly BogusMethod RANDOM_BOOL = new ("f => f.Random.Bool()");
        public static readonly BogusMethod RANDOM_INT = new ("f => f.Random.Int()");
        public static readonly BogusMethod RANDOM_DECIMAL = new ("f => f.Random.Decimal(0, 100)");
        public static readonly BogusMethod RANDOM_ENUM = new ("f => f.Random.Enum<{ENUM_NAME}>()");
        public static readonly BogusMethod RANDOM_FLOAT = new ("f => f.Random.Float()");

        public static readonly BogusMethod DATE_PAST = new("f => f.Date.Past()");
        public static readonly BogusMethod DATE_PAST_OFFSET = new("f => f.Date.PastOffset()");

        public static readonly BogusMethod INTERNET_PASSWORD = new ("f => f.Internet.Password()");

        public static readonly BogusMethod PERSON_FULL_NAME = new("f => f.Person.FullName");
        public static readonly BogusMethod PERSON_FIRST_NAME = new("f => f.Person.FirstName");
        public static readonly BogusMethod PERSON_LAST_NAME = new("f => f.Person.LastName");
        public static readonly BogusMethod PERSON_USER_NAME = new("f => f.Person.UserName");
        public static readonly BogusMethod PERSON_EMAIL = new("f => f.Person.Email");

        public static readonly BogusMethod COLLECTION = new("() => new {FAKER_NAME}().Generate(1)");
        public static readonly BogusMethod COMPLEX_OBJECT = new("new {FAKER_NAME}()");
        public static readonly BogusMethod ENTITY_OBJECT = new("new {FAKER_NAME}(context)");
    }
}
