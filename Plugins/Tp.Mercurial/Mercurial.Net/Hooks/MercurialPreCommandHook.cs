using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialControllingHookBase"/> descendant implements the
    /// code necessary to handle the "pre-command" hook:
    /// This is run before the command itself has started executing.
    /// </summary>
    /// <remarks>
    /// As with all controlling hooks (descendants of <see cref="MercurialControllingHookBase"/>), you can
    /// prevent the command from continuing, or let it continue, by calling
    /// <see cref="MercurialControllingHookBase.TerminateHookAndCancelCommand(int)"/>
    /// or <see cref="MercurialControllingHookBase.TerminateHookAndProceed"/> respectively.
    /// </remarks>
    public class MercurialPreCommandHook : MercurialControllingHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Arguments"/> property.
        /// </summary>
        private readonly MercurialCommandHookArgumentsCollection _Arguments = new MercurialCommandHookArgumentsCollection();

        /// <summary>
        /// This is the backing field for the <see cref="Options"/> property.
        /// </summary>
        private readonly MercurialCommandHookDictionary _Options = new MercurialCommandHookDictionary(Environment.GetEnvironmentVariable("HG_OPTS") ?? "{}");

        /// <summary>
        /// This is the backing field for the <see cref="Patterns"/> property.
        /// </summary>
        private readonly MercurialCommandHookPatternCollection _Patterns = new MercurialCommandHookPatternCollection(Environment.GetEnvironmentVariable("HG_PATS") ?? "[]");

        /// <summary>
        /// Gets the collection of arguments to the command.
        /// </summary>
        public MercurialCommandHookArgumentsCollection Arguments
        {
            get
            {
                return _Arguments;
            }
        }

        /// <summary>
        /// Gets the collection of options to the command.
        /// </summary>
        public MercurialCommandHookDictionary Options
        {
            get
            {
                return _Options;
            }
        }

        /// <summary>
        /// Gets the collection of patterns given to the command.
        /// </summary>
        public MercurialCommandHookPatternCollection Patterns
        {
            get
            {
                return _Patterns;
            }
        }
    }
}
