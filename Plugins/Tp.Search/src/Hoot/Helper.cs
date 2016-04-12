using System;
using System.Text;

namespace hOOt
{
	internal static class Helper
	{
		internal static int ToInt32(byte[] value, int startIndex, bool reverse)
		{
			if (reverse)
				Array.Reverse(value, startIndex, 4);
			return ToInt32(value, startIndex);
		}

		internal static unsafe int ToInt32(byte[] value, int startIndex)
		{
			fixed (byte* numRef = &(value[startIndex]))
			{
				return *((int*)numRef);
			}
		}

		internal static long ToInt64(byte[] value, int startIndex, bool reverse)
		{
			if (reverse)
				Array.Reverse(value, startIndex, 8);
			return ToInt64(value, startIndex);
		}

		internal static unsafe long ToInt64(byte[] value, int startIndex)
		{
			fixed (byte* numRef = &(value[startIndex]))
			{
				return *(((long*)numRef));
			}
		}

		internal static short ToInt16(byte[] value, int startIndex, bool reverse)
		{
			if (reverse)
				Array.Reverse(value, startIndex, 2);
			return ToInt16(value, startIndex);
		}

		internal static unsafe short ToInt16(byte[] value, int startIndex)
		{
			fixed (byte* numRef = &(value[startIndex]))
			{
				return *(((short*)numRef));
			}
		}

		internal static unsafe byte[] GetBytes(long num, bool reverse)
		{
			byte[] buffer = new byte[8];
			fixed (byte* numRef = buffer)
			{
				*((long*)numRef) = num;
			}
			if (reverse)
				Array.Reverse(buffer);
			return buffer;
		}

		internal static unsafe byte[] GetBytes(int num, bool reverse)
		{
			byte[] buffer = new byte[4];
			fixed (byte* numRef = buffer)
			{
				*((int*)numRef) = num;
			}
			if (reverse)
				Array.Reverse(buffer);
			return buffer;
		}

		internal static int Compare(bytearr left, bytearr right)
		{
			int lL = left.val.Length;
			int rL = right.val.Length;
			if (lL < rL)
				return -1;
			if (lL > rL)
				return 1;
			// key len equal
			int len = lL;

			for (int i = 0; i < len; i++)
			{
				int l = left.val[i];
				int r = right.val[i];
				int k = l - r;
				if (k == 0)
					continue;
				if (k < 0)
					return -1;
				else
					return 1;
			}
			return 0;
		}

		internal static byte[] GetBytes(string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		internal static string GetString(byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		internal static string GetString(byte[] buffer, int index, short length)
		{
			return Encoding.UTF8.GetString(buffer, index, length);
		}
	}
}