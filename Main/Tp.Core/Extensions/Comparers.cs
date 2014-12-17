using System;
using System.Collections.Generic;

namespace Tp.Core.Extensions
{
	public class StringCaseInsensitiveComparer : IEqualityComparer<string>
	{
		public static readonly StringCaseInsensitiveComparer Instance = new StringCaseInsensitiveComparer();

		private StringCaseInsensitiveComparer()
		{

		}

		public bool Equals(string x, string y)
		{
			return String.Compare(x, y, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public int GetHashCode(string obj)
		{
			return obj.ToLower().GetHashCode();
		}
	}

}