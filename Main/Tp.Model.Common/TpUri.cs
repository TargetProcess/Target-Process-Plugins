using System;
using Tp.Model.Common.Interfaces;

namespace Tp.Model.Common
{
    public class TpUri : ITpUri
    {
        protected bool Equals(TpUri other)
        {
            return string.Equals(ToString(), other.ToString());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TpUri) obj);
        }

        public override int GetHashCode()
        {
            return _uri?.GetHashCode() ?? 0;
        }

        private readonly string _uri;
        private readonly IApplicationPathFromRequestFirst _applicationPathFromRequestFirst;

        public TpUri(string uri, IApplicationPathFromRequestFirst applicationPathFromRequestFirst)
        {
            _uri = uri;
            _applicationPathFromRequestFirst = applicationPathFromRequestFirst;
        }

        public override string ToString()
        {
            try
            {
                return ToAbsoluteUri();
            }
            catch (UriFormatException)
            {
                return $"(invalid app Uri \"{_applicationPathFromRequestFirst.Value}\") {_uri}";
            }
        }

        public static string Combine(string path1, string path2)
        {
            var endsWith = path1.EndsWith("/");
            var startWith = path2.StartsWith("/");
            if (endsWith && startWith)
            {
                return path1 + path2.Substring(1);
            }
            if (endsWith)
            {
                return path1 + path2;
            }
            if (startWith)
            {
                return path1 + path2;
            }
            return path1 + "/" + path2;
        }

        private static string Combine(Uri path1, string path2)
        {
            return Combine(path1.AbsoluteUri, path2);
        }

        private string ToAbsoluteUri(string appPath)
        {
            return _uri.StartsWith("~") ? Combine(new Uri(appPath), _uri.Substring(1)) : _uri;
        }

        public string ToAbsoluteUri(Uri appPathUri)
        {
            return _uri.StartsWith("~") ? Combine(appPathUri, _uri.Substring(1)) : _uri;
        }

        public string ToAbsoluteUri()
        {
            return ToAbsoluteUri(_applicationPathFromRequestFirst.Value);
        }
    }
}
