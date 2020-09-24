using System;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using Tp.Core.Annotations;

namespace Tp.Core.Linq
{
    //Overloads for primitive types exist here, because C# expression builder cannot create method call expression with method that accepts object
    //and argument of ValueType. It throws 'Argument Type does not match' exception.
    //This methods are important for VizyDrop project.
    public static class SafeConvert
    {
        [PublicApiMethod("A special safe object to int conversion method for api/v2")]
        [CanBeNull]
        public static int? ToInt([CanBeNull] object value)
        {
            return ConvertTo(value, Convert.ToInt32);
        }

        [PublicApiMethod("A special safe decimal to int conversion method for api/v2")]
        [CanBeNull]
        public static int? ToInt([CanBeNull] decimal? value)
        {
            return ConvertTo(value, x => Convert.ToInt32(x));
        }

        [PublicApiMethod("A special safe bool to int conversion method for api/v2")]
        [CanBeNull]
        public static int? ToInt([CanBeNull] bool? value)
        {
            return ConvertTo(value, x => Convert.ToInt32(x));
        }

        [PublicApiMethod("A special safe object to boolean conversion method for api/v2")]
        [CanBeNull]
        public static bool? ToBoolean([CanBeNull] object value)
        {
            return ConvertTo(value, Convert.ToBoolean);
        }

        [PublicApiMethod("A special safe int to boolean conversion method for api/v2")]
        [CanBeNull]
        public static bool? ToBoolean([CanBeNull] int? value)
        {
            return ConvertTo(value, x => Convert.ToBoolean(x));
        }

        [PublicApiMethod("A special safe decimal to boolean conversion method for api/v2")]
        [CanBeNull]
        public static bool? ToBoolean([CanBeNull] decimal? value)
        {
            return ConvertTo(value, x => Convert.ToBoolean(x));
        }

        [PublicApiMethod("A special safe object to decimal conversion method for api/v2")]
        [CanBeNull]
        public static decimal? ToDecimal([CanBeNull] object value)
        {
            return ConvertTo(value, Convert.ToDecimal);
        }

        [PublicApiMethod("A special safe int to decimal conversion method for api/v2")]
        [CanBeNull]
        public static decimal? ToDecimal([CanBeNull] int? value)
        {
            return ConvertTo(value, x => Convert.ToDecimal(x));
        }

        [PublicApiMethod("A special safe bool to decimal conversion method for api/v2")]
        [CanBeNull]
        public static decimal? ToDecimal([CanBeNull] bool? value)
        {
            return ConvertTo(value, x => Convert.ToDecimal(x));
        }

        [PublicApiMethod("A special safe object to date conversion method for api/v2")]
        [CanBeNull]
        public static DateTime? ToDateTime([CanBeNull] object value)
        {
            return ConvertTo(value, Convert.ToDateTime);
        }

        [PublicApiMethod("A special safe string to date conversion method for api/v2")]
        [CanBeNull]
        public static DateTime? ToDateTime([CanBeNull] string value, string format)
        {
            return ConvertTo(value, x => DateTime.ParseExact(value, format, CultureInfo.InvariantCulture));
        }

        [PublicApiMethod("A special safe object to string conversion method for api/v2")]
        [CanBeNull]
        public static string ToString([CanBeNull] object value)
        {
            return ConvertReferenceToReferenceType(value, x => Convert.ToString(x));
        }

        [PublicApiMethod("A special safe int to string conversion method for api/v2")]
        [CanBeNull]
        public static string ToString([CanBeNull] int? value)
        {
            return ConvertValueToReferenceType(value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        [PublicApiMethod("A special safe int to string conversion method for api/v2")]
        [CanBeNull]
        public static string ToString([CanBeNull] int? value, string format)
        {
            return ConvertValueToReferenceType(value, x => x.ToString(format, CultureInfo.InvariantCulture));
        }

        [PublicApiMethod("A special safe decimal to string conversion method for api/v2")]
        [CanBeNull]
        public static string ToString([CanBeNull] decimal? value)
        {
            return ConvertValueToReferenceType(value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        [PublicApiMethod("A special safe decimal to string conversion method for api/v2")]
        [CanBeNull]
        public static string ToString([CanBeNull] decimal? value, string format)
        {
            return ConvertValueToReferenceType(value, x => x.ToString(format, CultureInfo.InvariantCulture));
        }

        [PublicApiMethod("A special safe bool to string conversion method for api/v2")]
        [CanBeNull]
        public static string ToString([CanBeNull] bool? value)
        {
            return ConvertValueToReferenceType(value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        [PublicApiMethod("A special safe date to string conversion method for api/v2")]
        [CanBeNull]
        public static string ToString([CanBeNull] DateTime? value)
        {
            return ConvertValueToReferenceType(value, x => x.ToString(CultureInfo.InvariantCulture));
        }

        [PublicApiMethod("A special safe date to string conversion method for api/v2")]
        [CanBeNull]
        public static string ToString([CanBeNull] DateTime? value, string format)
        {
            return ConvertValueToReferenceType(value, x => x.ToString(format, CultureInfo.InvariantCulture));
        }

        private static TTo? ConvertTo<TFrom, TTo>(TFrom value, Func<TFrom, TTo> conversionFunc) where TTo : struct
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                return conversionFunc(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static TTo ConvertReferenceToReferenceType<TFrom, TTo>(TFrom value, Func<TFrom, TTo> conversionFunc)
            where TFrom : class
            where TTo : class
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                return conversionFunc(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static TTo ConvertValueToReferenceType<TFrom,TTo>(TFrom? value, Func<TFrom, TTo> conversionFunc)
            where TFrom : struct
            where TTo : class
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                return conversionFunc(value.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [PublicApiMethod("A special safe object to array of ints conversion method for api/v2")]
        [CanBeNull]
        public static int?[] ToIntArray([CanBeNull] object value, char separator)
        {
            return ToTypedArray(value, separator, ToInt);
        }

        [PublicApiMethod("A special safe object to array of strings conversion method for api/v2")]
        [CanBeNull]
        public static string[] ToStringArray([CanBeNull] object value, char separator)
        {
            return ToTypedArray(value, separator, ToString);
        }

        [PublicApiMethod("A special safe object to array of decimals conversion method for api/v2")]
        [CanBeNull]
        public static decimal?[] ToDecimalArray([CanBeNull] object value, char separator)
        {
            return ToTypedArray(value, separator, ToDecimal);
        }

        [PublicApiMethod("A special safe object to array of booleans conversion method for api/v2")]
        [CanBeNull]
        public static bool?[] ToBooleanArray([CanBeNull] object value, char separator)
        {
            return ToTypedArray(value, separator, ToBoolean);
        }

        [PublicApiMethod("A special safe object to array of dates conversion method for api/v2")]
        [CanBeNull]
        public static DateTime?[] ToDateTimeArray([CanBeNull] object value, char separator)
        {
            return ToTypedArray(value, separator, ToDateTime);
        }

        [PublicApiMethod("A special safe object to array of dates conversion method for api/v2")]
        [CanBeNull]
        public static DateTime?[] ToDateTimeArray([CanBeNull] object value, char separator, string format)
        {
            return ToTypedArray(value, separator, x => ToDateTime(x, format));
        }

        private static T[] ToTypedArray<T>(object value, char separator, Func<string, T> converionFunc)
        {
            if (value == null)
            {
                return Array.Empty<T>();
            }

            try
            {
                var s = Convert.ToString(value);
                return string.IsNullOrEmpty(s) ? Array.Empty<T>() : s.Split(separator).Select(converionFunc).ToArray();
            }
            catch (Exception)
            {
                return Array.Empty<T>();
            }
        }
    }
}
