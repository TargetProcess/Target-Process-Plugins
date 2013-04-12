using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialHookBase"/> descendant implements the
    /// code necessary to handle the "pushkey" hook:
    /// This is run after a pushkey (like a bookmark) has been added to the repository.
    /// </summary>
    public class MercurialPushKeyHook : MercurialHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Namespace"/> property.
        /// </summary>
        private readonly string _Namespace = Environment.GetEnvironmentVariable("HG_NAMESPACE") ?? string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private readonly string _Name = Environment.GetEnvironmentVariable("HG_KEY") ?? string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="NewValue"/> property.
        /// </summary>
        private readonly string _NewValue = Environment.GetEnvironmentVariable("HG_NEW") ?? string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="OldValue"/> property.
        /// </summary>
        private readonly string _OldValue = Environment.GetEnvironmentVariable("HG_OLD") ?? string.Empty;

        /// <summary>
        /// Gets the namespace the key was pushed to.
        /// </summary>
        public string Namespace
        {
            get
            {
                return _Namespace;
            }
        }

        /// <summary>
        /// Gets the name of the key that was pushed.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        /// <summary>
        /// Gets the old value of the key.
        /// </summary>
        public string OldValue
        {
            get
            {
                return _OldValue;
            }
        }

        /// <summary>
        /// Gets the new value of the key.
        /// </summary>
        public string NewValue
        {
            get
            {
                return _NewValue;
            }
        }
    }
}