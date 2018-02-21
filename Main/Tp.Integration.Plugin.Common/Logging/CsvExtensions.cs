namespace Tp.Integration.Plugin.Common.Logging
{
    public static class CsvExtensions
    {
        public static string CsvEncode(this string source)
        {
            return source.Replace(@"""", @"""""");
        }
    }
}
