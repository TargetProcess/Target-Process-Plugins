using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tp.Utils.Csv
{
    public class CsvRenderer
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
        public CsvRenderer(char delimiter, TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException(nameof(textWriter));
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
                throw new ArgumentNullException(nameof(value));
            }
            if (!_newLineWritten)
            {
                _textWriter.Write(_delimiter);
            }
            WriteEscapedValue(value, _textWriter);
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

        private void WriteEscapedValue(string value, TextWriter textWriter)
        {
            if (value == string.Empty)
            {
                textWriter.Write("\"\"");
            }
            else
            {
                if (NeedEscape(value))
                {
                    textWriter.Write("\"");
                    foreach (char c in value)
                    {
                        if (c == '"')
                        {
                            textWriter.Write("\"\"");
                        }
                        else
                        {
                            textWriter.Write(c);
                        }
                    }
                    textWriter.Write("\"");
                }
                else
                {
                    textWriter.Write(value);
                }
            }
        }

        private bool NeedEscape(string value)
        {
            return value.Any(ch => ch == ',' || ch == '\n' || ch == '\r' || ch == '"' || ch == _delimiter);
        }

        public void Render(IEnumerable<string> columnNames)
        {
            foreach (String column in columnNames)
            {
                WriteValue(column);
            }
            StartNewLine();
        }
    }
}
