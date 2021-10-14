using Newtonsoft.Json;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class SerializerInfraService : ISerializerInfraService
    {
        public string SerializeIndented(object @object)
        {
            var jsonOptions = new JsonSerializerSettings { 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(@object, Formatting.Indented, jsonOptions);
        }
    }
}
