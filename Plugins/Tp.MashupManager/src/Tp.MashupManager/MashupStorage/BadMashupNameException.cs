using System;

namespace Tp.MashupManager.MashupStorage
{
	public class BadMashupNameException : Exception
	{
		public BadMashupNameException() { }
		public BadMashupNameException(string message) : base(message) { }
		public BadMashupNameException(string message, Exception innerException) : base(message, innerException) { }
	}
}