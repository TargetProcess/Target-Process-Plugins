using System.Net;
using Tp.Utils.Html;

namespace Tp.Search.Model.Utils
{
    internal class TextOperations
    {
        public string Prepare(string data)
        {
            var decoded = WebUtility.HtmlDecode(data);
            return PlainTextRenderer.RenderToPlainText(decoded).ToLower();
        }
    }
}
