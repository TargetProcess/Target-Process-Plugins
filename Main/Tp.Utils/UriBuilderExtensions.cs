// ReSharper disable CheckNamespace

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace System
// ReSharper restore CheckNamespace
{
	public static class UriBuilderExtensions
	{
		public static UriBuilder SetQueryParameters(this UriBuilder uri, NameValueCollection queryParameters)
		{
			var collection = uri.ParseQuery();

			foreach (var name in queryParameters.AllKeys)
			{
				collection.Set(name, queryParameters[name]);
			}

			uri.Query = collection.AsKeyValuePairs().Select(pair => pair.Key == null ? pair.Value : pair.Key + "=" + pair.Value).ToString("&");
			
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

		public static UriBuilder SetQueryParameter(this UriBuilder uri, string name, string value)
		{
			return uri.SetQueryParameters(new NameValueCollection { {name, value} });
		}

		public static IEnumerable<KeyValuePair<string, string>> GetQueryParams(this UriBuilder uri)
		{
			return uri.ParseQuery().AsKeyValuePairs();
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