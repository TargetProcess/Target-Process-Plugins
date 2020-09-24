using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tp.Utils.Csv
{
    public class CsvReader : IDisposable
    {
        private readonly TextReader _reader;
        private readonly char _delimiter;

        private int _line, _col;
        private readonly Dictionary<string, int> _columns = new Dictionary<string, int>();
        private readonly List<string> _fields = new List<string>();

        public CsvReader(TextReader reader)
            : this(reader, ',')
        {
        }

        public CsvReader(TextReader reader, char delimiter)
            : this(reader, delimiter, false)
        {
        }

        public CsvReader(TextReader reader, bool parseHeader)
            : this(reader, ',', parseHeader)
        {
        }

        public CsvReader(TextReader reader, char delimiter, bool parseHeader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _delimiter = delimiter;
            int t;
            if ((t = _reader.Read()) != -1)
            {
                _c = t;
            }
            if ((t = _reader.Read()) != -1)
            {
                _lac = t;
            }
            if (parseHeader)
            {
                if (Read())
                {
                    for (int i = 0; i < _fields.Count; i++)
                    {
                        _columns[_fields[i]] = i;
                    }
                }
            }
        }

        public string[] Fields => _fields.ToArray();

        private int _c = -1, _lac = -1;

        private void ReadChar()
        {
            if (_c != -1)
            {
                _c = _lac;
                int t;
                if ((t = _reader.Read()) != -1)
                {
                    _lac = t;
                }
                else
                {
                    _lac = -1;
                }
                _col++;
            }
        }

        public bool Read()
        {
            _fields.Clear();
            var s = new StringBuilder();

            bool comma = false;

            while (_c != -1)
            {
                // end of line?
                if (_c == '\n' || _c == '\r')
                {
                    _line++;
                    _col = 0;
                    if (_c == '\r' && _lac == '\n')
                    {
                        ReadChar();
                    }
                    ReadChar();
                    break;
                }

                // see if this is a quoted field value
                if (_c == '"')
                {
                    comma = false;

                    ReadChar();
                    while (_c != -1)
                    {
                        if (_c == '"')
                        {
                            if (_lac == '"')
                            {
                                s.Append('"');
                                ReadChar();
                            }
                            else
                            {
                                _fields.Add(s.ToString());
                                s.Length = 0;
                                ReadChar();
                                break;
                            }
                        }
                        else
                        {
                            s.Append((char) _c);
                        }
                        ReadChar();
                        if (_c == -1)
                        {
                            throw new MalformedCsvException("\" expected", _line, _col);
                        }
                    }

                    // skip white space at the end of quoted field value
                    while (_c == ' ' || (_delimiter != '\t' && _c == '\t'))
                    {
                        ReadChar();
                    }

                    // eat delimiter
                    if (_c == _delimiter)
                    {
                        comma = true;
                        ReadChar();
                    }
                    else
                    {
                        if (_c != -1 && _c != '\n' && _c != '\r')
                        {
                            throw new MalformedCsvException("Comma or new line expected", _line, _col);
                        }
                    }
                }
                else
                {
                    while (_c != -1)
                    {
                        comma = false;

                        // end of line?
                        if (_c == '\n' || _c == '\r')
                        {
                            _fields.Add(s.ToString());
                            s.Length = 0;
                            break;
                        }
                        if (_c == _delimiter)
                        {
                            _fields.Add(s.ToString());
                            s.Length = 0;
                            comma = true;
                            ReadChar();
                            break;
                        }
                        s.Append((char) _c);
                        ReadChar();
                    }
                }
            }
            if (s.Length > 0 || comma)
            {
                _fields.Add(s.ToString());
            }
            return _fields.Count > 0;
        }

        /// <summary>
        /// Get field at the specified position.
        /// </summary>
        /// <param name="index">
        /// Field position, starting from zero.
        /// </param>
        /// <returns>
        /// Field value at the specified position.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="index"/> is larger than number of columns in header.
        /// </exception>
        public string this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (_columns.Count > 0 && index > _columns.Count - 1)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (index < _fields.Count)
                {
                    return _fields[index];
                }
                return null;
            }
        }

        /// <summary>
        /// Get field of the column of the specified name,
        /// </summary>
        /// <param name="column">
        /// Column name.
        /// </param>
        /// <returns>
        /// Field value of the specified column.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="column"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If header has not been read.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="column"/> specifies unknown column.
        /// </exception>
        public string this[string column]
        {
            get
            {
                if (column == null)
                {
                    throw new ArgumentNullException(nameof(column));
                }
                if (_columns.Count == 0)
                {
                    throw new InvalidOperationException("Header has not been read");
                }
                if (_columns.ContainsKey(column))
                {
                    int index = _columns[column];
                    if (index < _fields.Count)
                    {
                        return _fields[index];
                    }
                    return null;
                }
                throw new ArgumentException($"Unknown column name \"{column}\"");
            }
        }

        public int Line => _line;

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
