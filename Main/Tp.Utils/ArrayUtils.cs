using System.Linq;

namespace Tp.Utils
{
	public static class ArrayUtils
	{
		public static int[] Add(int[] a, int[] b)
		{
			return a.Select((x, i) => x + b[i]).ToArray();
		}

		public static int[] Subtract(int[] a, int[] b)
		{
			return a.Select((x, i) => x - b[i]).ToArray();
		}
	}
}
