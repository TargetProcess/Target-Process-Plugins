using System;

namespace Tp.Core.MaybeConvert
{
	public static class MaybeConvertExtensions
	{
		public static Maybe<int> MaybeToInt(this string s)
		{
			if (!s.IsNullOrEmpty())
			{
				int result;
				if (int.TryParse(s, out result))
					return result.ToMaybe();
			}

			return Maybe.Nothing;
		}
	}
}