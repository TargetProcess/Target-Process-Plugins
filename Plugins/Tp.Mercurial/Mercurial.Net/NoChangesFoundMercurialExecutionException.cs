using System;
using System.Runtime.Serialization;

namespace Mercurial
{
    /// <summary>
    /// Represents errors related to Mercurial.
    /// </summary>
    [Serializable]
    public class NoChangesFoundMercurialExecutionException : MercurialExecutionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref = "NoChangesFoundMercurialExecutionException" /> class.
        /// </summary>
        /// <param name="info">The <see cref = "T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref = "T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref = "T:System.ArgumentNullException">
        /// The <paramref name = "info" /> parameter is null.
        /// </exception>
        /// <exception cref = "T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref = "P:System.Exception.HResult" /> is zero (0).
        /// </exception>
        protected NoChangesFoundMercurialExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "NoChangesFoundMercurialExecutionException" /> class.
        /// </summary>
        public NoChangesFoundMercurialExecutionException()
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "NoChangesFoundMercurialExecutionException" /> class
        /// with a specific error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public NoChangesFoundMercurialExecutionException(string message)
            : base(message)
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "NoChangesFoundMercurialExecutionException" /> class
        /// with a specific error message.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the Mercurial command line client executable.
        /// </param>
        /// <param name="message">The message.</param>
        public NoChangesFoundMercurialExecutionException(int exitCode, string message)
            : base(exitCode, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "NoChangesFoundMercurialExecutionException" /> class
        /// s with a specified error message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception,
        /// or a <c>null</c> reference (<c>Nothing</c> in Visual Basic)
        /// if no inner exception is specified. 
        /// </param>
        public NoChangesFoundMercurialExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "NoChangesFoundMercurialExecutionException" /> class
        /// s with a specified error message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the Mercurial command line client executable.
        /// </param>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception,
        /// or a <c>null</c> reference (<c>Nothing</c> in Visual Basic)
        /// if no inner exception is specified. 
        /// </param>
        public NoChangesFoundMercurialExecutionException(int exitCode, string message, Exception innerException)
            : base(exitCode, message, innerException)
        {
        }
    }
}