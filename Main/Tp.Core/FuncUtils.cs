using System;
using Tp.Core.Annotations;

namespace Tp.Core
{
    public static class FuncUtils
    {
        /// <summary>
        /// Dumb helper function to initialize field with anonymous delegate.
        /// </summary>
        public static TResult SelfInvoke<TResult>([NotNull] [InstantHandle] Func<TResult> f)
        {
            return f();
        }
    }
}
