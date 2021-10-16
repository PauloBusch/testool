using System.Collections;
using System.Collections.Generic;
using TesTool.Core.Models.Templates.Factory;

namespace TesTool.Core.Interfaces.Services.Factories
{
    public interface IFactoryModelService
    {
        ModelFactory GetModelFactory(string name);
    }
}
