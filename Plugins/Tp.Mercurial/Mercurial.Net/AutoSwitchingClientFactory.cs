using System;
using System.Diagnostics;

namespace Mercurial
{
    /// <summary>
    /// This class implements <see cref="IClientFactory"/> by constructing either a
    /// <see cref="NonPersistentClient"/> or <see cref="PersistentClient"/>
    /// depending on circumstances, and also implements <see cref="IClientSwitcher"/>
    /// to allow for switching before or after certain commands.
    /// </summary>
    public class AutoSwitchingClientFactory : IClientFactory, IClientSwitcher
    {
        /// <summary>
        /// Creates a new <see cref="IClient"/> object for use by the <see cref="Repository"/>.
        /// </summary>
        /// <param name="repositoryPath">
        /// The path to the repository to manage by the <see cref="IClient"/> and the
        /// <see cref="Repository"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IClient"/> implementation to use.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repositoryPath"/> is <c>null</c> or empty.</para>
        /// </exception>
        public IClient CreateClient(string repositoryPath)
        {
            if (PersistentClient.IsSupported(repositoryPath))
                return new PersistentClient(repositoryPath);

            return new NonPersistentClient(repositoryPath);
        }

        /// <summary>
        /// Determines if the client should be switched out before the specific command is
        /// executed. If it is switched out, the old client should be disposed of, and a new client
        /// should be returned. If it is not switched out, the current client should be simply returned.
        /// </summary>
        /// <param name="command">
        /// The command that is about to be executed.
        /// </param>
        /// <param name="currentClient">
        /// The current <see cref="IClient"/> implementation.
        /// </param>
        /// <returns>
        /// The <see cref="IClient"/> to use to execute the command, and from now on.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="currentClient"/> is <c>null</c>.</para>
        /// </exception>
        public IClient SwitchBeforeCommand(IMercurialCommand command, IClient currentClient)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (currentClient == null)
                throw new ArgumentNullException("currentClient");

            return currentClient;
        }

        /// <summary>
        /// Determines if the client should be switched out after the specific command has
        /// executed. If it is switched out, the old client should be disposed of, and a new client
        /// should be returned. If it is not switched out, the current client should be simply returned.
        /// This method is only called upon successful execution of the command.
        /// </summary>
        /// <param name="command">
        /// The command that was successfully executed.
        /// </param>
        /// <param name="currentClient">
        /// The current <see cref="IClient"/> implementation.
        /// </param>
        /// <returns>
        /// The <see cref="IClient"/> to use from now on.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="currentClient"/> is <c>null</c>.</para>
        /// </exception>
        public IClient SwitchAfterCommand(IMercurialCommand command, IClient currentClient)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (currentClient == null)
                throw new ArgumentNullException("currentClient");

            if (!(command is InitCommand))
                return currentClient;
            if (!(currentClient is NonPersistentClient))
                return currentClient;

            if (PersistentClient.IsSupported(currentClient.RepositoryPath))
            {
                string repositoryPath = currentClient.RepositoryPath;
                var disposable = currentClient as IDisposable;
                if (disposable != null)
                    disposable.Dispose();

                return new PersistentClient(repositoryPath);
            }

            return currentClient;
        }
    }
}