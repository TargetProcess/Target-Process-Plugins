using System;

namespace Tp.Model.Common.Exceptions
{
    /// <summary>
    /// This exception is thrown when a generic custom field error occurs.
    /// </summary>
    [Serializable]
    public abstract class CustomFieldException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// A message that describes the error. 
        /// </param>
        protected CustomFieldException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldException" /> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception. 
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. 
        /// If the <paramref name="innerException" /> parameter is not a null reference, 
        /// the current exception is raised in a catch block that handles the inner exception. 
        /// </param>
        protected CustomFieldException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
