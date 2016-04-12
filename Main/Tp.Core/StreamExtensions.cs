using System;
using System.IO;
using System.Text;

namespace Tp.Core
{
	public static class StreamExtensions
	{
		public static string ReadAllThenResetPositionToZero(this Stream stream, Encoding encoding = null)
		{
			if (!stream.CanSeek)
			{
				throw new ArgumentException("Stream must support seeking.", nameof(stream));
			}
			using (Disposable.Create(() => stream.Position = 0))
			{
				var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
				return reader.ReadToEnd();
			}
		}
	}
}
