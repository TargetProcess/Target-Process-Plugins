using System;
using System.Threading;

namespace Mercurial
{
    /// <summary>
    /// This class implements <see cref="IAsyncResult{T}"/> by wrapping an
    /// instance of <see cref="IAsyncResult"/>, and being generic to be
    /// a type-safe way to track asynchronous execution.
    /// </summary>
    /// <typeparam name="T">
    /// The type of executing the command asynchronously.
    /// </typeparam>
    internal class AsyncResult<T> : IAsyncResult<T>
    {
        /// <summary>
        /// This is the backing field for the <see cref="InnerResult"/> property.
        /// </summary>
        private readonly IAsyncResult _InnerResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResult{T}"/> class.
        /// </summary>
        /// <param name="innerResult">
        /// The inner <see cref="IAsyncResult"/> object to wrap.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="innerResult"/> is <c>null</c>.</para>
        /// </exception>
        public AsyncResult(IAsyncResult innerResult)
        {
            if (innerResult == null)
                throw new ArgumentNullException("innerResult");

            _InnerResult = innerResult;
        }

        #region IAsyncResult<T> Members

        /// <summary>
        /// Gets the inner <see cref="IAsyncResult"/> object.
        /// </summary>
        public IAsyncResult InnerResult
        {
            get
            {
                return _InnerResult;
            }
        }

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A user-defined object that qualifies or contains information about an asynchronous operation.
        /// </returns>
        public object AsyncState
        {
            get
            {
                return _InnerResult.AsyncState;
            }
        }

        /// <summary>
        /// Gets a <see cref="WaitHandle"/> that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <returns>
        /// A <see cref="WaitHandle"/> that is used to wait for an asynchronous operation to complete.
        /// </returns>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return _InnerResult.AsyncWaitHandle;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <returns>
        /// true if the asynchronous operation completed synchronously; otherwise, false.
        /// </returns>
        public bool CompletedSynchronously
        {
            get
            {
                return _InnerResult.CompletedSynchronously;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the asynchronous operation has completed.
        /// </summary>
        /// <returns>
        /// true if the operation is complete; otherwise, false.
        /// </returns>
        public bool IsCompleted
        {
            get
            {
                return _InnerResult.CompletedSynchronously;
            }
        }

        #endregion
    }
}