using System;

namespace Tp.Core.Diagnostics.Time
{
	public class HttpHeaderAttribute : Attribute, ITextProvider
	{
		private readonly string _header;

		public HttpHeaderAttribute(string header)
		{
			_header = header;
		}

		public string GetText()
		{
			return _header;
		}
	}
}
