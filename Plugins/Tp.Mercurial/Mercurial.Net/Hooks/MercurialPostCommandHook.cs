using System;
using System.Globalization;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialControllingHookBase"/> descendant implements the
    /// code necessary to handle the "post-command" hook:
    /// This is run after the command itself has finished executing.
    /// </summary>
    public class MercurialPostCommandHook : MercurialControllingHookBase
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
        /// This is the backing field for the <see cref="ExitCode"/> property.
        /// </summary>
        private readonly int _ExitCode = int.Parse(Environment.GetEnvironmentVariable("HG_RESULT") ?? "0", CultureInfo.InvariantCulture);

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
        /// Gets the exit code of the Mercurial command that finished executing.
        /// </summary>
        public int ExitCode
        {
            get
            {
                return _ExitCode;
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
