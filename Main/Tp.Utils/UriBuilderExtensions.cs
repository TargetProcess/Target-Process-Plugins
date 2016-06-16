using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace System
// ReSharper restore CheckNamespace
{
	public static class UriBuilderExtensions
	{
		internal const string OriginHeader = "Origin";
		internal const string ProxySslModeHeader = "X-Ssl-Proxy";
		internal const string ProxyPortHeader = "X-Proxy-Port";
		internal const string ProxyAlternativeHostHeader = "X-Alternative-Host";

		public static UriBuilder SetQueryParameters(this UriBuilder uri, NameValueCollection queryParameters, bool skipEmptyValues = false)
		{
			var collection = uri.ParseQuery();

			foreach (var name in queryParameters.AllKeys)
			{
				collection.Set(name, queryParameters[name]);
			}

			var pairs = collection.AsKeyValuePairs();
			if (skipEmptyValues)
			{
				pairs = pairs.Where(x => !string.IsNullOrEmpty(x.Value));
			}


			uri.Query = pairs
				.Select(pair => pair.Key == null ? pair.Value : pair.Key + "=" + pair.Value)
				.ToString("&");

			return uri;
		}

		public static UriBuilder SetQueryParameters(this UriBuilder uri, object data)
		{
			uri.Query = string.Join("&", data.GetType()
				.GetProperties()
				.Select(x => new { Key = x.Name, Value = x.GetValue(data) })
				.Where(x => x.Value != null)
				.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value.ToString())));

			return uri;
		}

		public static UriBuilder SetQueryParameter(this UriBuilder uri, string name, string value, bool skipNullValues = false)
		{
			return uri.SetQueryParameters(new NameValueCollection { { name, value } });
		}

		public static IEnumerable<KeyValuePair<string, string>> GetQueryParams(this UriBuilder uri)
		{
			return uri.ParseQuery().AsKeyValuePairs();
		}

		public static UriBuilder PatchFromHeaders(this UriBuilder baseUri, NameValueCollection headers)
		{
			var port = headers[ProxyPortHeader].ParseInt() ?? baseUri.Port;
			var isSsl = baseUri.IsSsl(headers);
			if (isSsl)
			{
				baseUri.Scheme = Uri.UriSchemeHttps;
				baseUri.Port = port == 443 ? -1 : port;
			}
			else
			{
				baseUri.Scheme = Uri.UriSchemeHttp;
				baseUri.Port = port == 80 ? -1 : port;
			}
			var alternative = headers[ProxyAlternativeHostHeader];
			if (!alternative.IsNullOrEmpty())
			{
				baseUri.Host = alternative.Trim();
			}
			var origin = headers[OriginHeader];
			return !origin.IsNullOrEmpty() ? PrepareFromOrigin(origin, baseUri) : baseUri;
		}

		public static bool IsSsl(this UriBuilder url, NameValueCollection headers)
		{
			return IsSsl(url.Scheme, headers);
		}

		public static bool IsSsl(this Uri url, NameValueCollection headers)
		{
			return IsSsl(url.Scheme, headers);
		}

		private static bool IsSsl(string scheme, NameValueCollection headers)
		{
			if (scheme == Uri.UriSchemeHttps)
			{
				return true;
			}
			// if this header is set then we nginx proxied
			return !string.IsNullOrEmpty(headers[ProxySslModeHeader]);
		}

		private static UriBuilder PrepareFromOrigin(string origin, UriBuilder baseUri)
		{
			try
			{
				var originUri = new Uri(origin);
				var isSsl = originUri.Scheme == Uri.UriSchemeHttps;
				var port = originUri.Port;
				baseUri.Scheme = originUri.Scheme;
				baseUri.Port = isSsl && port == 443 || !isSsl && port == 80 ? -1 : port;
				baseUri.Host = originUri.Host;
				return baseUri;
			}
			catch (UriFormatException)
			{
				baseUri.Host = origin;
			}
			return baseUri;
		}

		private static IEnumerable<KeyValuePair<string, string>> AsKeyValuePairs(this NameValueCollection collection)
		{
			return collection.AllKeys.Select(key => new KeyValuePair<string, string>(key, collection.Get(key)));
		}

		private static NameValueCollection ParseQuery(this UriBuilder uri)
		{
			return HttpUtility.ParseQueryString(uri.Query);
		}
	}
}
