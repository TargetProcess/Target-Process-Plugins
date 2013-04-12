using System;

namespace Mercurial
{
    /// <summary>
    /// This class implements <see cref="IClientFactory"/> by always constructing objects
    /// of type <see cref="NonPersistentClient"/>.
    /// </summary>
    public sealed class NonPersistentClientFactory : IClientFactory
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
            return new NonPersistentClient(repositoryPath);
        }
    }
}
