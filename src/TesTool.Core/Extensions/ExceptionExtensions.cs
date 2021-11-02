using System;

namespace TesTool.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessageString(this Exception exception)
        {
            var error = $"{exception.GetType().Name}: {exception.Message}";
            var inner = exception.InnerException;

            while (inner != null)
            {
                error = $"{error} - Inner - {inner.GetType().Name}: {inner.Message}";
                inner = inner.InnerException;
            }

            return error;
        }
    }
}
