using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tp.Search.Model.Document
{
	static class IndexDataStringServices
	{
		private const string PartsSeparator = " ";
		public static string OfParts(IEnumerable<string> parts)
		{
			return string.Join(PartsSeparator, parts.Where(x => !string.IsNullOrEmpty(x)));
		}

		public static IEnumerable<string> ToParts(string s)
		{
			return s.Split(new[] { PartsSeparator }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string EncodeStringId(int? id, string prefix)
		{
			if (id == null)
			{
				return prefix + "null";
			}
			var array = new BitArray(new[] { id.Value });
			var list = new List<string>();
			foreach (bool f in array)
			{
				list.Add(f ? "t" : "f");
			}
			list.Reverse();
			var result = list.Aggregate(new StringBuilder(), (acc, x) => acc.Append(x));
			return prefix + result;
		}

		public static int? DecodeStringId(string value, string prefix)
		{
			string valueWithoutPrefix = value.Replace(prefix, string.Empty).Replace(prefix.ToLower(), string.Empty);
			if (string.Equals("null", valueWithoutPrefix, StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}
			var convertedValue = valueWithoutPrefix.Replace("t", "1").Replace("f","0");
			return Convert.ToInt32(convertedValue, 2);
		}
	}
}