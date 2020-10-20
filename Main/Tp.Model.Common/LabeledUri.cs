using System;
using System.Text.RegularExpressions;
using Tp.Core;
using Tp.Core.Annotations;

// ReSharper disable once CheckNamespace

namespace Tp.BusinessObjects
{
    /// <summary>
    /// A <see cref="Uri"/> with custom label.
    /// </summary>
    [Serializable]
    public sealed class LabeledUri : IEquatable<LabeledUri>
    {
        private const string URL_EXP_RFC3986 =
            @"(?<url>([a-z][a-z0-9+\-.]*:(//([a-z0-9\-._~%!$&'()*+,;=]+@)?([a-z0-9\-._~%]+|\[[a-f0-9:.]+\]|\[v[a-f0-9][a-z0-9\-._~%!$&'()*+,;=:]+\])(:[0-9]+)?(/[a-z0-9\-._~%!$&'()*+,;=:@]+)*/?|(/?[a-z0-9\-._~%!$&'()*+,;=:@]+(/[a-z0-9\-._~%!$&'()*+,;=:@]+)*/?)?)|([a-z0-9\-._~%!$&'()*+,;=@]+(/[a-z0-9\-._~%!$&'()*+,;=:@]+)*/?|(/[a-z0-9\-._~%!$&'()*+,;=:@]+)+/?))(\?[a-z0-9\-._~%!$&'()*+,;=:@/?]*)?(\#[a-z0-9\-._~%!$&'()*+,;=:@/?]*)?$)";

        static LabeledUri()
        {
            _regexRfc3986 = new Regex(URL_EXP_RFC3986, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private static readonly Regex _regexRfc3986;

        private readonly string _label;

        /// <summary>
        /// Creates new instance of this class.
        /// </summary>
        /// <param name="label">Uri label. If <code>null</code>, then uri will be used as label.</param>
        /// <param name="uri">Uri.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="uri"/> is <c>null</c>.</exception>
        public LabeledUri([CanBeNull] string label, [NotNull] Uri uri)
        {
            Argument.NotNull(nameof(uri), uri);
            Uri = uri;
            _label = label.TrimToNull();
            if (_label == Uri.ToString())
            {
                _label = null;
            }
        }

        /// <summary>
        /// Creates new instance of this class.
        /// </summary>
        /// <param name="label">Uri label. If <code>null</code>, then uri will be used as label.</param>
        /// <param name="uri">Uri string.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="uri"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException">If cannot parse <paramref name="uri"/> string.</exception>
        public LabeledUri([CanBeNull] string label, [NotNull] string uri) : this(label, new Uri(uri))
        {
        }

        [NotNull]
        public string Label => _label ?? Uri.ToString();

        [NotNull]
        public Uri Uri { get; }

        //Done for compatibility with api/v1
        [NotNull]
        [UsedImplicitly]
        public Uri Url => Uri;

        public static LabeledUri Parse(string value)
        {
            return !TryParse(value, out LabeledUri result) ? new LabeledUri(null, value) : result;
        }

        public static bool TryParse(string value, out LabeledUri result)
        {
            result = null;
            if (value == null)
            {
                return false;
            }

            string url;
            string description = null;
            value = value.Replace(" ", "%20");

            if (_regexRfc3986.IsMatch(value))
            {
                url = _regexRfc3986.Match(value).Groups["url"].Value;
                var d = value.Substring(0, value.IndexOf(url, StringComparison.Ordinal));
                if (!d.IsNullOrEmpty())
                {
                    d = d.Trim();
                    if (!d.IsNullOrEmpty())
                    {
                        description = d;
                    }
                }
            }
            else
            {
                var split = value.Split(StringUtils.LineBreaks, StringSplitOptions.RemoveEmptyEntries);

                switch (split.Length)
                {
                    case 2:
                        description = split[0];
                        url = split[1];
                        break;
                    case 1:
                        url = split[0];
                        break;
                    default:
                        return false;
                }
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                return false;

            result = new LabeledUri(string.IsNullOrEmpty(description) ? description : description.Replace("%20", " "), uri);
            return true;
        }

        #region Operators

        public static implicit operator Uri(LabeledUri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return uri.Uri;
        }

        public static implicit operator LabeledUri(Uri uri)
        {
            return new LabeledUri("", uri);
        }

        #endregion

        #region Object Methods

        /// <summary>
        /// Two LabeledUri objects are considered equal if they have equal labels and URIs.
        /// URI comparison should take URI fragment into account, which is ignored in standard
        /// implementation of Uri.Equals.
        ///
        /// Possible implementation ways:
        /// - comparison of Uri.OriginalString instead of Uri.Equals
        /// - comparison of Uri.fragment in addition to Uri.Equals
        /// - call to static Uri.Compare which allows to specify URI parts participating in comparison
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public bool Equals(LabeledUri that)
        {
            if (that == null)
            {
                return false;
            }

            if (!Equals(that.Label, Label))
            {
                return false;
            }

            return Equals(that.Uri.ToString(), Uri.ToString());
        }

        public override bool Equals(object that)
        {
            return Equals(that as LabeledUri);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Label.GetHashCode() * 397) ^ Uri.ToString().GetHashCode();
            }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public override string ToString()
        {
            return $"{Label}\n{(Uri.IsAbsoluteUri ? Uri.AbsoluteUri : Uri.ToString())}";
        }

        #endregion
    }

    public static class LabeledUriExtensions
    {
        [NotNull]
        public static string ToHref([NotNull] this LabeledUri uri, bool openInNewWindow = false)
        {
            return
                $"<a href='{uri.Uri}'{(openInNewWindow ? " target=\'_blank\' rel=\'noopener noreferrer\'" : string.Empty)}>{uri.Label}</a>";
        }
    }
}
