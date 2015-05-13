using System.Globalization;

namespace System
{
	public static class NumberExtensions
	{
		public static string ToStringInvariant(this int i)
		{
			return i.ToString(CultureInfo.InvariantCulture);
		}

		public static string ToStringInvariant(this int? i)
		{
			if (!i.HasValue)
			{
				throw new ArgumentNullException("i");
			}
			return i.Value.ToStringInvariant();
		}

		public static string ToStringInvariantSafe(this int? i)
		{
			if (!i.HasValue)
			{
// ReSharper disable once ExpressionIsAlwaysNull
				return i.ToString();
			}
			return i.Value.ToStringInvariant();
		}
	}
}