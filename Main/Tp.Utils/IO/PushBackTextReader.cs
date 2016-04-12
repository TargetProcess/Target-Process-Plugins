using System;
using System.IO;
using System.Text;

namespace Tp.Utils.IO
{
	/// <summary>
	/// Not a real text reader, but still is very handy.
	/// </summary>
	public sealed class PushBackTextReader : /*TextReader,*/ IDisposable
	{
		private const int Bof = -2;
		private const int Eof = -1;

		private readonly TextReader _reader;

		private readonly StringBuilder _buf = new StringBuilder();

		private int _readPos;

		private int _current = Bof;

		public PushBackTextReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			_reader = reader;
		}

		public bool IsBof
		{
			get { return _current == Bof; }
		}

		public bool IsEof
		{
			get { return _current == Eof; }
		}

		public char Current
		{
			get
			{
				if (_current == Bof)
				{
					throw new InvalidOperationException("Read has not been called yet");
				}
				if (_current == Eof)
				{
					throw new InvalidOperationException("Cannot read past end of stream");
				}
				return (char) _current;
			}
		}

		public bool Read()
		{
			if (_buf.Length > 0)
			{
				if (_readPos < _buf.Length)
				{
					_current = _buf[_readPos];
					_readPos++;
					return true;
				}
				_buf.Length = _readPos = 0;
			}
			_current = _reader.Read();
			return _current != Eof;
		}

		public bool Read(int count)
		{
			while (count > 0)
			{
				if (!Read())
				{
					return false;
				}
				count--;
			}
			return true;
		}

		public void Push(char s)
		{
			_buf.Insert(_readPos, s);
		}

		public void Push(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException(nameof(s));
			}
			if (s.Length == 0)
			{
				throw new ArgumentException("Cannot push empty string");
			}
			_buf.Insert(_readPos, s);
		}

		public bool Match(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException(nameof(s));
			}
			if (s.Length == 0)
			{
				throw new ArgumentException("Cannot match empty string");
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (_buf.Length < i + _readPos + 1)
				{
					int n = _reader.Read();
					if (n != Eof)
					{
						_buf.Append((char) n);
					}
					else
					{
						return false;
					}
				}
				if (_buf[i + _readPos] != s[i])
				{
					return false;
				}
			}
			return true;
		}

		public void Dispose()
		{
			_reader.Dispose();
		}
	}
}
