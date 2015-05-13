using System;

namespace Tp.Utils.Sys
{
    public class ProcessTimeOutException:ApplicationException
    {
        /// <summary>
        /// Creates new instance of this class.
        /// </summary>
        /// <param name="message">Error message</param>
        public ProcessTimeOutException(string message) : base(message) {}
        /// <summary>
        /// Creates new instance of this class.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ProcessTimeOutException(string message, Exception innerException) : base(message, innerException) {}


    }
}