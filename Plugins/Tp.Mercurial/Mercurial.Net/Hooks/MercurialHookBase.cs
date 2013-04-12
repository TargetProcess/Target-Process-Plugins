using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This is the base class for Mercurial hook type implementations.
    /// </summary>
    public class MercurialHookBase : IDisposable
    {
        /// <summary>
        /// This is the backing field for the <see cref="Repository"/> property.
        /// </summary>
        private readonly Repository _Repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialHookBase"/> class.
        /// </summary>
        protected MercurialHookBase()
        {
            ClientExecutable.SetClientPath(Environment.GetEnvironmentVariable("HG"));
            _Repository = new Repository(Environment.CurrentDirectory, new NonPersistentClientFactory());
        }

        /// <summary>
        /// Gets the <see cref="Repository"/> the hook is executing in.
        /// </summary>
        public Repository Repository
        {
            get
            {
                return _Repository;
            }
        }

        /// <summary>
        /// Loads a <see cref="RevSpec"/> from a hash specified by
        /// an environment variable.
        /// </summary>
        /// <param name="environmentVariableName">
        /// The name of the environment variable to load the <see cref="RevSpec"/> from.
        /// </param>
        /// <returns>
        /// The loaded <see cref="RevSpec"/>;
        /// or <c>null</c> if the environment variable did not exist or contained
        /// an empty string.
        /// </returns>
        protected static RevSpec LoadRevision(string environmentVariableName)
        {
            var leftParentHash = Environment.GetEnvironmentVariable(environmentVariableName);
            if (StringEx.IsNullOrWhiteSpace(leftParentHash))
                return null;

            return new RevSpec(leftParentHash);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _Repository.Dispose();
        }
    }
}