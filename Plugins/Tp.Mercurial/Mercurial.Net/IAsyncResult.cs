using System;

namespace Mercurial
{
    /// <summary>
    /// This interface will be returned from <see cref="Repository.BeginExecute{T}"/> in order to
    /// produce a type-safe asynchronous execution.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result of executing the command asynchronously.
    /// </typeparam>
// ReSharper disable UnusedTypeParameter
    public interface IAsyncResult<T> : IAsyncResult
// ReSharper restore UnusedTypeParameter
    {
        /// <summary>
        /// Gets the inner <see cref="IAsyncResult"/> object.
        /// </summary>
        IAsyncResult InnerResult
        {
            get;
        }
    }
}