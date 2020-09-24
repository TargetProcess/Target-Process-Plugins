using System.Web;

namespace Tp.Utils
{
    public static class RequestExtensions
    {
        private const string XHttpMethodOverrideHeader = "X-HTTP-Method-Override";

        public static string ResolveHttpMethod(this HttpRequestBase request)
        {
            return request.Headers[XHttpMethodOverrideHeader] ?? request.HttpMethod;
        }

        public static string ResolveHttpMethod(this HttpRequest request)
        {
            return request.Headers[XHttpMethodOverrideHeader] ?? request.HttpMethod;
        }
    }
}
