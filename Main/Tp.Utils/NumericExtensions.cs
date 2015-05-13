// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class NumericExtensions
	{
		public static int? ParseInt(this string source)
		{
			int result;

			if (Int32.TryParse(source, out result))
			{
				return result;
			}

			return null;
		}

		public static int ParseAsInt32(this byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}

			if (bytes.Length != 4)
			{
				throw new ArgumentException("Input byte array should have 4 bytes", "bytes");
			}

			var result = 0;

			result = (result << 8) + bytes[3];
			result = (result << 8) + bytes[2];
			result = (result << 8) + bytes[1];
			result = (result << 8) + bytes[0];

			return result;
		}
	}
}
