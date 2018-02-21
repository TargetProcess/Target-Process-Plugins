using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Tp.Bugzilla.BugzillaQueries
{
    public class BugzillaCustomFieldAction : IBugzillaAction
    {
        private static readonly Regex LineBreakTagRegEx = new Regex(@"(<br\ *\/?>)+", RegexOptions.Compiled);
        private static readonly Regex HtmlTagsRegEx = new Regex(@"<[^>]*>", RegexOptions.Compiled);

        private readonly string _bugzillaBugId;
        private readonly string _customFieldName;
        private readonly string _customFieldVaue;

        public BugzillaCustomFieldAction(string bugzillaBugId, string customFieldName, string customFieldVaue)
        {
            _bugzillaBugId = bugzillaBugId;
            _customFieldName = customFieldName;
            _customFieldVaue = customFieldVaue;
        }

        public string Value()
        {
            var plainText = StripHtmlTags(_customFieldVaue);
            var decoded = HttpUtility.HtmlDecode(plainText);
            //remove &nbsp;
            var text = decoded.Replace((char) 160, ' ');

            var base64String = HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(text)));

            return $"cmd=add_customfield&bugid={_bugzillaBugId}&cf_name={_customFieldName}&cf_value={base64String}";
        }

        public string GetOperationDescription()
        {
            return $"Set bug with id '{_bugzillaBugId}' custom field '{_customFieldName}' value to '{_customFieldVaue}'";
        }

        private static string StripHtmlTags(string input)
        {
            return HtmlTagsRegEx.Replace(LineBreakTagRegEx.Replace(input, "\r\n"), string.Empty);
        }
    }
}
