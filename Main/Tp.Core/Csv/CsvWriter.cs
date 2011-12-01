// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Tp.Utils.Csv
{
	/// <summary>
	/// Writes CSV files.
	/// </summary>
	public class CsvWriter
	{
		private readonly char _delimiter;
		private readonly TextWriter _textWriter;
		private bool _newLineWritten = true;

		/// <summary>
		/// Creates new instance of this class.
		/// </summary>
		/// <param name="delimiter">Delimiter to separate columns.</param>
		/// <param name="textWriter">Where to write CSV text.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="textWriter"/> is <c>null</c>.</exception>
		public CsvWriter(char delimiter, TextWriter textWriter)
		{
			if (textWriter == null)
			{
				throw new ArgumentNullException("textWriter");
			}
			_delimiter = delimiter;
			_textWriter = textWriter;
		}

		public void WriteBlock(StringBuilder stream)
		{
			_textWriter.Write(stream.ToString());
			_newLineWritten = false;
			StartNewLine();
		}

		/// <summary>
		/// Writes array of values as a row in CSV file.
		/// </summary>
		/// <param name="values">Values to write in a row in CSV file.</param>
		/// <exception cref="ArgumentException">If <paramref name="values"/> is empty array.</exception>
		/// <exception cref="ArgumentException">If <paramref name="values"/> contains nulls.</exception>
		public void WriteLine(params string[] values)
		{
			if (values.Length == 0)
			{
				throw new ArgumentException("Values cannot be empty");
			}
			if (values.Any(s => s == null))
			{
				throw new ArgumentException("Values cannot contain null");
			}
			if (!_newLineWritten)
			{
				StartNewLine();
			}
			foreach (string s in values)
			{
				WriteValue(s);
			}
			StartNewLine();
		}

		/// <summary>
		/// Write single value in the current row at the current cursor position.
		/// </summary>
		/// <param name="value">Single value to write at the current cursor position.</param>
		public void WriteValue(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!_newLineWritten)
			{
				_textWriter.Write(_delimiter);
			}
			WriteEscapedValue(value);
			_newLineWritten = false;
		}

		/// <summary>
		/// Wraps value writing cursor to new line.
		/// </summary>
		/// <exception cref="InvalidOperationException">If new line has laready been started.</exception>
		public void StartNewLine()
		{
			if (_newLineWritten)
			{
				throw new InvalidOperationException("New line has already been started");
			}
			_textWriter.WriteLine();
			_newLineWritten = true;
		}

		private void WriteEscapedValue(string value)
		{
			if (value == "")
			{
				_textWriter.Write("\"\"");
			}
			else
			{
				if (NeedEscape(value))
				{
					_textWriter.Write("\"");
					foreach (char c in value)
					{
						if (c == '"')
						{
							_textWriter.Write("\"\"");
						}
						else
						{
							_textWriter.Write(c);
						}
					}
					_textWriter.Write("\"");
				}
				else
				{
					_textWriter.Write(value);
				}
			}
		}

		private bool NeedEscape(string value)
		{
			return value.Any(ch => ch == ',' || ch == '\n' || ch == '\r' || ch == '"' || ch == _delimiter);
		}

		public void RenderDataTable(DataTable data)
		{
			RenderHeader(data);
			RenderRows(data.Rows.Cast<DataRow>());
		}

		public void RenderHeader(DataTable data)
		{
			foreach (DataColumn column in data.Columns)
			{
				WriteValue(column.ColumnName);
			}
			StartNewLine();
		}

		public void RenderRows(IEnumerable<DataRow> dataRowCollection)
		{
			foreach (var row in dataRowCollection)
			{
				RenderRow(row);
			}
		}

		public void RenderRow(DataRow row)
		{
			foreach (object value in row.ItemArray)
			{
				WriteValue(Convert.ToString(value));
			}
			StartNewLine();
		}
	}
}