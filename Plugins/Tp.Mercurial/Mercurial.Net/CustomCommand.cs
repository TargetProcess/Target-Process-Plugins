using System;

namespace Mercurial
{
    /// <summary>
    /// This class implements execution of a custom Mercurial command.
    /// </summary>
    public sealed class CustomCommand : IncludeExcludeCommandBase<CustomCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCommand"/> class.
        /// </summary>
        /// <param name="command">
        /// The command to execute through Mercurial.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c> or empty.</para>
        /// </exception>
        public CustomCommand(string command)
            : base(command)
        {
            // Do nothing here
        }
    }
}
