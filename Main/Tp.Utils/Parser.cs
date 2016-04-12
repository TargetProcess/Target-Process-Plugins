using System;

namespace Tp.Components
{
	public static class Parser
	{
		private const string WWW_PREFIX = "WWW.";

		public static string ParseHostFromEmailAddress(string emailAddress)
		{
			int emailIndex = emailAddress.IndexOf('@');
			return emailAddress.Substring(emailIndex + 1);
		}

		public static string ParseHostFromUrl(string url, bool withWwwPrefix)
		{
			var uri = new Uri(url);
			string hostName = uri.Host;
			if (withWwwPrefix)
			{
				return hostName;
			}
			int indexWww = hostName.IndexOf(WWW_PREFIX, StringComparison.InvariantCultureIgnoreCase);
			if (indexWww == 0)
			{
				return hostName.Substring(WWW_PREFIX.Length);
			}
			return hostName;
		}
	}
}
