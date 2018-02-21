using System.Web;
using Tp.Core.Features;

namespace Tp.Utils
{
    public static class VerbosityExtensions
    {
        private static bool IsVerbose(this HttpRequestBase httpRequest)
        {
            var isVerbose = httpRequest.QueryString["verbose"]?.Equals("1");

#if DEBUG
            isVerbose = true;
#endif

            return isVerbose == true;
        }

        public static bool IncludeStackTracesInResponse(this HttpRequestBase httpRequest)
        {
            if (!httpRequest.IsVerbose())
            {
                return false;
            }

            return TpFeature.IncludeStackTracesInResponse.IsEnabled();
        }

        public static bool IncludeStackTracesInResponse(this HttpRequest httpRequest)
        {
            return new HttpRequestWrapper(httpRequest).IncludeStackTracesInResponse();
        }
    }
}