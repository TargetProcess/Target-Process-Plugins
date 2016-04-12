using Tp.I18n;

namespace Tp.Core
{
	public class InvalidDateRangeException : LocalizedInvalidOperationException
	{
		public InvalidDateRangeException(IFormattedMessage message)
			: base(message)
		{
		}
	}
}
