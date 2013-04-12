using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialControllingHookBase"/> descendant implements the
    /// code necessary to handle the "prepushkey" hook:
    /// This is run before a pushkey (like a bookmark) is added to the repository.
    /// </summary>
    /// <remarks>
    /// As with all controlling hooks (descendants of <see cref="MercurialControllingHookBase"/>), you can
    /// prevent the command from continuing, or let it continue, by calling
    /// <see cref="MercurialControllingHookBase.TerminateHookAndCancelCommand(int)"/>
    /// or <see cref="MercurialControllingHookBase.TerminateHookAndProceed"/> respectively.
    /// </remarks>
    public class MercurialPrePushKeyHook : MercurialControllingHookBase
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