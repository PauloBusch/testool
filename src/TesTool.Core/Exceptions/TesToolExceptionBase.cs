using System;

namespace TesTool.Core.Exceptions
{
    public class TesToolExceptionBase : Exception
    {
        public TesToolExceptionBase(string message) : base(message) { }

        public TesToolExceptionBase(string message, Exception innerException) : base(message, innerException) { }
    }
}
