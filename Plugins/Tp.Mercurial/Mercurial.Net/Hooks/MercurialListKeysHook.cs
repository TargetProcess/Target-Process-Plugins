using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialHookBase"/> descendant implements the
    /// code necessary to handle the "listkeys" hook:
    /// This is run after listing pushkeys (like bookmarks) in the repository.
    /// </summary>
    public class MercurialListKeysHook : MercurialHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Namespace"/> property.
        /// </summary>
        private readonly string _Namespace = Environment.GetEnvironmentVariable("HG_NAMESPACE") ?? string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Values"/> property.
        /// </summary>
        private readonly MercurialCommandHookDictionary _Values = new MercurialCommandHookDictionary(Environment.GetEnvironmentVariable("HG_VALUES") ?? "{}");

        /// <summary>
        /// Gets the namespace the keys was listed from.
        /// </summary>
        public string Namespace
        {
            get
            {
                return _Namespace;
            }
        }

        /// <summary>
        /// Gets the dictionary containing the keys and values that was listed.
        /// </summary>
        public MercurialCommandHookDictionary Values
        {
            get
            {
                return _Values;
            }
        }
    }
}