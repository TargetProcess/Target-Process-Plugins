using System;

namespace Tp.Search.Model.Exceptions
{
    class DocumentIndexConcurrentAccessException : ApplicationException
    {
        public DocumentIndexConcurrentAccessException()
        {
        }

        public DocumentIndexConcurrentAccessException(string message) : base(message)
        {
        }

        public DocumentIndexConcurrentAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
