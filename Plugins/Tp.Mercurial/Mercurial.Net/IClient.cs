using System;

namespace Mercurial
{
    /// <summary>
    /// This interface is used by Mercurial.Net to manage replacable client
    /// wrapper implementations.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Executes the given <see cref="IMercurialCommand"/> command against
        /// the Mercurial repository.
        /// </summary>
        /// <param name="command">
        /// The <see cref="IMercurialCommand"/> command to execute.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="MercurialException">
        /// HG did not complete within the allotted time.
        /// </exception>
        void Execute(IMercurialCommand command);

        /// <summary>
        /// Gets the path to the repository (or not-yet-repository) that the client
        /// is managing.
        /// </summary>
        string RepositoryPath
        {
            get;
        }

        /// <summary>
        /// Stops command executing.
        /// </summary>
        void CancelExecuting();
    }
}