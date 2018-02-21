using System;

namespace Tp.Core
{
    public class RootUri
    {
        private readonly Uri _root;

        public RootUri(Uri root)
        {
            _root = root;
        }

        public Uri JoinRelativeUri(string relativeUri)
        {
            relativeUri = relativeUri.TrimStart('/');
            return new Uri(_root, new Uri(relativeUri, UriKind.Relative));
        }
    }
}
