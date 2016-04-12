using Tp.I18n;

namespace System.Linq.Dynamic
{
	public sealed class ParseException : Exception, IFormattedMessageContainer
	{
		public ParseException(IFormattedMessage message, int position)
			: base(message.Value)
		{
			Position = position;
			FormattedMessage = message;
		}

		public ParseException(Exception innerException, int position) : base(innerException.Message, innerException)
		{
			Position = position;
			FormattedMessage = innerException.Message.AsLocalized();
		}

		public int Position { get; }

		public override string ToString()
		{
			return $"{Message} (at index {Position})";
		}

		public IFormattedMessage FormattedMessage { get; }
	}
}
