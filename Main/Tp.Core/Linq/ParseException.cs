namespace System.Linq.Dynamic
{
	public sealed class ParseException : Exception
	{
		private readonly int _position;

		public ParseException(string message, int position)
			: base(message)
		{
			_position = position;
		}

		public ParseException(Exception innerException, int position) : base(innerException.Message, innerException)
		{
			_position = position;
		}

		public int Position
		{
			get { return _position; }
		}

		public override string ToString()
		{
			return string.Format(Res.ParseExceptionFormat, Message, _position);
		}
	}
}