namespace Tp.Core.Csv
{
	public static class CsvExtensions
	{
		public static string CsvEncode(this string source)
		{
			return source.Replace(@"""", @"""""");
		}
	}
}
