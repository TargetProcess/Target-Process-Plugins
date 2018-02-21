using System;
using System.Collections.Generic;
using System.Text;

namespace Tp.Core
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<Exception> Flatten(this Exception exception)
        {
            for (Exception error = exception; error != null; error = error.InnerException)
            {
                yield return error;
            }
        }
    }
}
