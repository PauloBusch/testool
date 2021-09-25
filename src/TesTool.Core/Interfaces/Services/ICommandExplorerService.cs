using System;
using System.Collections.Generic;

namespace TesTool.Core.Interfaces.Services
{
    public interface ICommandExplorerService
    {
        Type GetDefaultCommandType();
        Type GetCommandTypeExact(string[] args);
        IEnumerable<Type> GetCommandTypesMatched(string[] args);
        IEnumerable<Type> GetAllCommandTypes();
    }
}
