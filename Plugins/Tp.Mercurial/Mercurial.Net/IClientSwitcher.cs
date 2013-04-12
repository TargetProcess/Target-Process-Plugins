using System;

namespace Mercurial
{
    /// <summary>
    /// This interface is used by <see cref="Repository"/> to determine if the current
    /// <see cref="IClient"/> should be switched out with a new client before or after
    /// executing a specific command. The object implementing <see cref="IClientFactory"/>
    /// can optionally implement this interface to automate this process.
    /// </summary>
    public interface IClientSwitcher
    {
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
        IClient SwitchBeforeCommand(IMercurialCommand command, IClient currentClient);

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
        IClient SwitchAfterCommand(IMercurialCommand command, IClient currentClient);
    }
}