using System;

namespace Mercurial
{
    /// <summary>
    /// This interface must be implemented by classes that will produce <see cref="IClient"/>
    /// implementations.
    /// </summary>
    public interface IClientFactory
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
        IClient CreateClient(string repositoryPath);
    }
}
