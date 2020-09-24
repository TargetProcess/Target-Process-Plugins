using System;

namespace Tp.Core.MaybeConvert
{
    public static class MaybeConvertExtensions
    {
        public static Maybe<int> MaybeToInt(this string s)
        {
            if (!s.IsNullOrEmpty() && int.TryParse(s, out var result))
            {
                return Maybe.Just(result);
            }

            return Maybe.Nothing;
        }
    }
}
