using System;
using System.Collections.Generic;

namespace TesTool.Core.Interfaces.Services
{
    public interface ICommandExplorerService
    {
        Type GetCommandTypeExact(string[] arguments);
        IEnumerable<Type> GetCommandTypesMatched(string[] arguments);
        IEnumerable<Type> GetAllCommandTypes();
    }
}
