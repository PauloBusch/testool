using System;

namespace TesTool.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessageString(this Exception exception)
        {
            var error = exception.Message;
            var inner = exception.InnerException;

            while (inner != null)
            {
                error = $"\n{inner.Message}";
                inner = inner.InnerException;
            }

            return error;
        }
    }
}
