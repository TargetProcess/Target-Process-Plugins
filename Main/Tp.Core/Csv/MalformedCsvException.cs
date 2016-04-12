using System;

namespace Tp.Utils.Csv
{
	/// <summary>
	/// Represents the exception that is thrown when a CSV file is malformed.
	/// </summary>
	[Serializable]
	public class MalformedCsvException : ApplicationException
	{
		#region Fields

		/// <summary>
		/// Contains the row index of the line.
		/// </summary>
		private readonly int _line;

		/// <summary>
		/// Contains the column index of the position at which the error occured.
		/// </summary>
		private readonly int _column;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the MalformedCsvException class.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="line">The row index of the line.</param>
		/// <param name="column">The column index of the position at which the error occured.</param>
		public MalformedCsvException(string message, int line, int column)
			: this(message, line, column, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MalformedCsvException class.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="line">The row index of the line.</param>
		/// <param name="column">The column index of the position at which the error occured.</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public MalformedCsvException(string message, int line, int column, Exception innerException)
			: base(message, innerException)
		{
			_line = line;
			_column = column;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the row index of the line.
		/// </summary>
		/// <value>The row index of the line.</value>
		public int Line
		{
			get { return _line; }
		}

		/// <summary>
		/// Gets the column index of the position at which the error occured.
		/// </summary>
		/// <value>The column index of the position at which the error occured.</value>
		public int Column
		{
			get { return _column; }
		}

		#endregion

		#region Overrides

		/// <summary>
		/// <see cref="System.Exception.Message"/>.
		/// </summary>
		public override string Message
		{
			get { return string.Format("The CSV appears to be corrupt near line {0}, column {1}: {2}", _line, _column, base.Message); }
		}

		#endregion
	}
}
