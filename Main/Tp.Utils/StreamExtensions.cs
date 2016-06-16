namespace System.IO
{
	public static class StreamExtensions
	{
		// This will be used in copying input stream to output stream.
		public const int ReadStreamBufferSize = 64 * 1024;

		public static void CopyTo(this Stream source, Stream destination)
		{
			source.CopyTo(destination, 0, source.Length, () => { });
		}

		public static void CopyTo(this Stream source, Stream destination, long start, long length, Action chunkAction)
		{
			var end = start + length;
			source.Seek(start, SeekOrigin.Begin);
			var bytesRemaining = end - source.Position;
			var buffer = new byte[ReadStreamBufferSize];

			while (bytesRemaining > 0)
			{
				var bytesRead = source.Read(buffer, 0, bytesRemaining > ReadStreamBufferSize ? ReadStreamBufferSize : (int) bytesRemaining);

				if (bytesRead == 0)
					break;

				destination.Write(buffer, 0, bytesRead);
				chunkAction();

				bytesRemaining = end - source.Position;
			}
		}

		public static byte[] ToArray(this Stream stream)
		{
			MemoryStream memoryStream;
			if (stream is MemoryStream)
				memoryStream = (MemoryStream) stream;
			else
			{
				memoryStream = new MemoryStream();
				stream.CopyTo(memoryStream);
			}
			return memoryStream.ToArray();
		}

		public static Stream ToStream(this byte[] body)
		{
			return new MemoryStream(body);
		}
	}
}
