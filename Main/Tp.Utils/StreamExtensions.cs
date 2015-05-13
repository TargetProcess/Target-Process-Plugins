namespace System.IO
{
	public static class StreamExtensions
	{
		public static void CopyTo(this Stream source, Stream destination)
		{
			source.CopyTo(destination, new byte[64*1024], ()=> { });
		}

		public static void CopyTo(this Stream source, Stream destination, byte[] buffer, Action chunkAction)
		{
			var length = buffer.Length;
			while (true)
			{
				int read = source.Read(buffer, 0, length);
				if (read <= 0)
					break;

				destination.Write(buffer, 0, read);
				chunkAction();
			}
		}

		public static byte[] ToArray(this Stream stream)
		{
			MemoryStream memoryStream;
			if (stream is MemoryStream)
				memoryStream = (MemoryStream)stream;
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