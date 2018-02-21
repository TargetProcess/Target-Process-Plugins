using System;

namespace Tp.Utils
{
    public static class TypeConverter
    {
        public static object Convert(object value, Type convertionType)
        {
            if (convertionType == typeof(DateTime) && (value == null || value as string == ""))
            {
                return null;
            }

            if (convertionType == typeof(bool) && (value == null || value as string == ""))
            {
                return false;
            }

            return System.Convert.ChangeType(value, convertionType);
        }
    }
}
