using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Tp.Search.Model.Utils
{
	internal class TextOperations
	{
		private readonly Regex _imgRegex = new Regex(@"<img[^>]+>", RegexOptions.Singleline | RegexOptions.Compiled);
		private readonly Regex _htmlRegex = new Regex(@"<(.|\n)*?>", RegexOptions.Singleline | RegexOptions.Compiled);
		private readonly Regex _newLinesRegex = new Regex(@"\s+", RegexOptions.Singleline | RegexOptions.Compiled);

		public string Prepare(string s)
		{
			return HttpUtility.HtmlDecode(StripHtmlTagsAndToLower(s));
		}
			
		private string StripHtmlTagsAndToLower(string origText)
		{
			return String.IsNullOrEmpty(origText) ? String.Empty : _newLinesRegex.Replace(_htmlRegex.Replace(_imgRegex.Replace(origText, " "), " "), " ").ToLower();
		}
	}
}