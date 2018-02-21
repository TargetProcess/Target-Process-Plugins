using System;

namespace Tp.MashupManager.MashupStorage
{
    public class BadMashupFileNameException : Exception
    {
        public BadMashupFileNameException()
        {
        }

        public BadMashupFileNameException(string message) : base(message)
        {
        }

        public BadMashupFileNameException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
