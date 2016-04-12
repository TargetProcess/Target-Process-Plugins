using System.IO;
using System.Text;

namespace Tp.Utils.IO
{
	/// <summary>
	/// Redirects its output to the specified array of text writer.
	/// </summary>
	public class TextWriterSplitter : TextWriter
	{
		private readonly TextWriter[] _writers;

		/// <summary>
		/// Create new instance of text writer splitter.
		/// </summary>
		/// <param name="writers">Array of writers.</param>
		public TextWriterSplitter(params TextWriter[] writers)
		{
			_writers = writers;
		}

		public override Encoding Encoding
		{
			get { return Encoding.Unicode; }
		}

		public override void Write(char value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(char[] buffer)
		{
			foreach (var writer in _writers)
			{
				writer.Write(buffer);
			}
		}

		public override void Write(char[] buffer, int index, int count)
		{
			foreach (var writer in _writers)
			{
				writer.Write(buffer, index, count);
			}
		}

		public override void Write(bool value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(int value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(uint value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(long value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(ulong value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(float value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(double value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(decimal value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(string value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(object value)
		{
			foreach (var writer in _writers)
			{
				writer.Write(value);
			}
		}

		public override void Write(string format, object arg0)
		{
			foreach (var writer in _writers)
			{
				writer.Write(format, arg0);
			}
		}

		public override void Write(string format, object arg0, object arg1)
		{
			foreach (var writer in _writers)
			{
				writer.Write(format, arg0, arg1);
			}
		}

		public override void Write(string format, object arg0, object arg1, object arg2)
		{
			foreach (var writer in _writers)
			{
				writer.Write(format, arg0, arg1, arg2);
			}
		}

		public override void Write(string format, params object[] arg)
		{
			foreach (var writer in _writers)
			{
				writer.Write(format, arg);
			}
		}

		public override void Flush()
		{
			foreach (var writer in _writers)
			{
				writer.Flush();
			}
		}

		public override void Close()
		{
			foreach (var writer in _writers)
			{
				writer.Close();
			}
		}

		protected override void Dispose(bool disposing)
		{
			foreach (var writer in _writers)
			{
				writer.Dispose();
			}
		}
	}
}
