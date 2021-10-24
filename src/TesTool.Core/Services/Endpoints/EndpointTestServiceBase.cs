using System.Collections.Generic;
using System.Linq;
using TesTool.Core.Enumerations;
using TesTool.Core.Extensions;
using TesTool.Core.Models.Enumerators;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Services.Endpoints
{
    public abstract class EndpointTestServiceBase
    {
        protected readonly RequestMethod _requestMethod;

        protected EndpointTestServiceBase(RequestMethod requestMethod)
        {
            _requestMethod = requestMethod;
        }

        protected RequestCall GetRequestCall(Endpoint endpoint)
        {
            // TODO: map return type
            return new RequestCall(
                endpoint.Route, _requestMethod.Name, default,
                GetInputBodyClass(endpoint.Inputs)?.Name.ToLowerCaseFirst(),
                GetInputQueryClass(endpoint.Inputs)?.Name.ToLowerCaseFirst()
            );
        }

        protected IEnumerable<FakerDeclaration> GetInputModels(IEnumerable<Input> inputs)
        {
            var inputBody = GetInputBodyClass(inputs);
            var inputQuery = GetInputQueryClass(inputs);
            if (inputBody is not null) yield return new FakerDeclaration(inputBody.Name);
            if (inputQuery is not null) yield return new FakerDeclaration(inputBody.Name);
        }

        private Class GetInputBodyClass(IEnumerable<Input> inputs) => GetInputClass(inputs, InputSourceEnumerator.BODY);
        private Class GetInputQueryClass(IEnumerable<Input> inputs) => GetInputClass(inputs, InputSourceEnumerator.QUERY);

        private Class GetInputClass(IEnumerable<Input> inputs, InputSource inputSource)
        {
            return inputs.FirstOrDefault(i => i.Source == inputSource)?.Type as Class;
        }
    }
}
