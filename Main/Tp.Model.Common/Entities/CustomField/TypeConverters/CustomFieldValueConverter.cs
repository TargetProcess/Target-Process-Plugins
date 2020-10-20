using System;
using System.Globalization;
using System.Linq;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Core.Features;
using Tp.Core.MaybeConvert;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;
using Tp.Model.Common.Entities.CustomField;
using Tp.Model.Common.Exceptions;
using Tp.Model.Common.Interfaces;
using NumberFormatInfo = System.Globalization.NumberFormatInfo;

// ReSharper disable once CheckNamespace

namespace Tp.BusinessObjects
{
    /// <summary>
    /// The class is responsible for conversion of values to string
    /// with condition that sort order of converted string is same to sort order of original number values.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code>
    ///  var converter = new CustomFieldValueConverter();
    ///  var input = 1234.22003d;
    ///  var output = converter.ConvertValueObjectToString(source, FieldTypeEnum.Money);
    /// //integer part in output string will be filled with zeros until length lower than value of IntegerPartWidth field
    /// </code>
    /// </example>
    /// </remarks>
    public static class CustomFieldValueConverter
    {
        private const NumberStyles NumberAndMoneyNumberStyle = NumberStyles.Number | NumberStyles.AllowExponent;
        public static readonly int IntegerPartWidth = 20;

        /// <summary>
        /// Get unparsed value.
        /// </summary>
        /// <param name="value">value.</param>
        /// <param name="type">Custom field type enum.</param>
        /// <returns>Unparsed value string.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="type"/> is of wrong value.</exception>
        [CanBeNull]
        [ContractAnnotation("value:null => null")]
        [Pure]
        public static string ConvertObjectValueToUnparsedString(
            [CanBeNull] object value, FieldTypeEnum type)
        {
            if (value == null)
            {
                return null;
            }

            switch (type)
            {
                case FieldTypeEnum.Text:
                case FieldTypeEnum.RichText:
                case FieldTypeEnum.DropDown:
                case FieldTypeEnum.MultipleSelectionList:
                case FieldTypeEnum.TemplatedURL:
                case FieldTypeEnum.Entity:
                case FieldTypeEnum.MultipleEntities:
                case FieldTypeEnum.CheckBox:
                case FieldTypeEnum.URL:
                    return Convert.ToString(value);
                case FieldTypeEnum.Number:
                case FieldTypeEnum.Money:
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
                case FieldTypeEnum.Date:
                    return ((DateTime) value).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                default:
                    throw GetInvalidFieldTypeException(type);
            }
        }

        /// <summary>
        /// Convert a custom field value to its text representation for database.
        /// </summary>
        /// <exception cref="CustomFieldTypeMismatchException">If <paramref name="value"/> is of wrong type.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="type"/> is of wrong value.</exception>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is negative.</exception>
        [CanBeNull]
        [ContractAnnotation("value:null => null")]
        [Pure]
        public static string ConvertValueObjectToString(
            [CanBeNull] object value, FieldTypeEnum type)
        {
            if (value == null || Equals(value, string.Empty))
            {
                return null;
            }

            switch (type)
            {
                case FieldTypeEnum.Text:
                case FieldTypeEnum.RichText:
                case FieldTypeEnum.DropDown:
                case FieldTypeEnum.MultipleSelectionList:
                case FieldTypeEnum.TemplatedURL:
                    if (!(value is string))
                    {
                        throw CustomFieldTypeMismatchException.Create(typeof(string), value.GetType());
                    }
                    return (string) value;
                case FieldTypeEnum.CheckBox:
                    if (!(value is bool))
                    {
                        throw CustomFieldTypeMismatchException.Create(typeof(bool), value.GetType());
                    }
                    var b = (bool) value;
                    return b ? "true" : "false";
                case FieldTypeEnum.Number:
                case FieldTypeEnum.Money:
                    return ConvertBoxedNumberToString(value);
                case FieldTypeEnum.Entity:
                    switch (value)
                    {
                        case IIdHolder idHolder:
                            return Convert.ToString(idHolder.ID);
                        case byte _:
                        case sbyte _:
                        case short _:
                        case ushort _:
                        case int _:
                        case uint _:
                        case long _:
                        case ulong _:
                            return Convert.ToString(value);
                        default:
                            throw CustomFieldTypeMismatchException.Create("integer", value.GetType().Name);
                    }

                case FieldTypeEnum.Date:
                    if (!(value is DateTime))
                    {
                        throw CustomFieldTypeMismatchException.Create(nameof(DateTime), value.GetType().Name);
                    }
                    var dateTime = (DateTime) value;
                    return Convert.ToString(dateTime.Ticks).PadLeft(IntegerPartWidth, '0');
                case FieldTypeEnum.URL:
                    if (!(value is LabeledUri) && !(value is Uri))
                    {
                        throw new CustomFieldTypeMismatchException(
                            "Invalid value type, expected '{firstExpected}' or '{anotherExpected}', but got '{actual}'.".Localize(new
                            {
                                firstExpected = nameof(LabeledUri),
                                anotherExpected = nameof(Uri),
                                actual = value.GetType().Name,
                            }));
                    }
                    var uri = value as LabeledUri ?? (Uri) value; // make sure LabeledUri goes first, the order of type casts matter!
                    return Convert.ToString(uri);
                case FieldTypeEnum.MultipleEntities:
                    if (value is string stringValue)
                    {
                        return stringValue;
                    }

                    if (!(value is object[] ids))
                    {
                        throw CustomFieldTypeMismatchException.Create(typeof(string), value.GetType());
                    }
                    return string.Join(",", ids.Select(id => id.ToString()));
                default:
                    throw GetInvalidFieldTypeException(type);
            }
        }

        /// <summary>
        /// Get value to be presented on the user interface.
        /// </summary>
        /// <param name="value">Custom field value.</param>
        /// <param name="type">Custom field type enum.</param>
        /// <param name="config">Custom field format config.</param>
        /// <returns>Custom field value of proper type.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="type"/> is of wrong value.</exception>
        /// <exception cref="FormatException">If <paramref name="value"/> is not consistent with <paramref name="type"/>.</exception>
        [CanBeNull]
        [Pure]
        public static object ConvertStringToValueObject(
            [NotNull] string value, FieldTypeEnum type, IFormattableCustomFieldConfig config)
        {
            switch (type)
            {
                case FieldTypeEnum.Text:
                case FieldTypeEnum.RichText:
                case FieldTypeEnum.DropDown:
                case FieldTypeEnum.MultipleSelectionList:
                case FieldTypeEnum.TemplatedURL:
                case FieldTypeEnum.MultipleEntities:
                    return value;
                case FieldTypeEnum.CheckBox:
                    return bool.Parse(value);
                case FieldTypeEnum.Number:
                    return decimal.Parse(value, NumberAndMoneyNumberStyle, CultureInfo.InvariantCulture);
                case FieldTypeEnum.Money:
                    return ParseMoneyWithFormat(value, CreateMoneyFormatProvider(config));
                case FieldTypeEnum.Entity:
                    return value.MaybeToInt()
                        .OrElse(() =>
                        {
                            var splitted = value.Split('\n');
                            return splitted.Length >= 2 ? splitted[1].Trim().MaybeToInt() : Maybe<int>.Nothing;
                        })
                        .ToNullable();
                case FieldTypeEnum.Date:
                    return Maybe.FromTryOut<DateTime>(DateTime.TryParse, value)
                        .OrElse(() => Maybe.FromTryOut<long>(long.TryParse, value).Select(x => new DateTime(x)))
                        .Select(x =>
                        {
                            if (TpFeature.DatesWithTZForCF.IsEnabled())
                            {
                                switch (x.Kind)
                                {
                                    case DateTimeKind.Unspecified:
                                        return DateTime.SpecifyKind(x, DateTimeKind.Local);
                                    case DateTimeKind.Utc:
                                    case DateTimeKind.Local:
                                        return x;
                                    default: throw new ArgumentOutOfRangeException(nameof(x.Kind));
                                }
                            }

                            return x;
                        })
                        .Select(x => x.Date)
                        .ToNullable();
                case FieldTypeEnum.URL:
                    return LabeledUri.Parse(value);
                default:
                    throw GetInvalidFieldTypeException(type);
            }
        }

        private static Maybe<IFormatProvider> CreateMoneyFormatProvider(IFormattableCustomFieldConfig formatConfig)
        {
            if (formatConfig.FormatSpecifier == null || formatConfig.FormatInfo == null
                || !TpFeature.AllowSpecifyCustomMoneyFormat.IsEnabled())
            {
                return Maybe.Nothing;
            }

            var nfi = (Tp.Integration.Messages.Entities.NumberFormatInfo) formatConfig.FormatInfo;
            return Maybe.Return<IFormatProvider>(new NumberFormatInfo
            {
                NumberDecimalDigits = nfi.CurrencyDecimalDigits,
                NumberDecimalSeparator = nfi.CurrencyDecimalSeparator,
                NumberGroupSeparator = nfi.CurrencyGroupSeparator,
                NumberGroupSizes = nfi.CurrencyGroupSizes
            });
        }

        private static decimal ParseMoneyWithFormat(string value, Maybe<IFormatProvider> formatProvider)
        {
            return formatProvider.HasValue && decimal.TryParse(value, NumberAndMoneyNumberStyle, formatProvider.Value, out var money)
                ? money
                : ParseMoneyWithInvariantFormat();

            decimal ParseMoneyWithInvariantFormat() => decimal.Parse(value, NumberAndMoneyNumberStyle, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get value to be presented on the history records.
        /// </summary>
        /// <param name="value">Custom field value.</param>
        /// <param name="type">Custom field type enum.</param>
        /// <param name="config">Custom field config</param>
        /// <returns>Custom field value of proper type.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="type"/> is of wrong value.</exception>
        /// <exception cref="FormatException">If <paramref name="value"/> is not consistent with <paramref name="type"/>.</exception>
        [CanBeNull]
        [Pure]
        public static object ConvertStringToHistoryRecord(
            [NotNull] string value, FieldTypeEnum type, [CanBeNull] CustomFieldConfig config)
        {
            switch (type)
            {
                case FieldTypeEnum.Text:
                case FieldTypeEnum.RichText:
                case FieldTypeEnum.DropDown:
                case FieldTypeEnum.MultipleSelectionList:
                case FieldTypeEnum.MultipleEntities:
                case FieldTypeEnum.TemplatedURL:
                case FieldTypeEnum.CheckBox:
                    return value;
                case FieldTypeEnum.Number:
                    return $"{value:G29}";
                case FieldTypeEnum.Money:
                {
                    return
                        decimal.TryParse(value, NumberAndMoneyNumberStyle, CultureInfo.InvariantCulture, out var parsedDecimal)
                            .MaybeAs<bool>()
                            .Bind<bool, string>(v => v ? ConvertMoneyToHistoryRecord(parsedDecimal, config) : string.Empty)
                            .GetOrDefault();
                }
                case FieldTypeEnum.Entity:
                    return value.MaybeToInt()
                        .OrElse(() =>
                        {
                            var splitted = value.Split('\n');
                            return splitted.Length >= 2 ? splitted[1].Trim().MaybeToInt() : Maybe<int>.Nothing;
                        })
                        .ToNullable();
                case FieldTypeEnum.Date:
                    return Maybe.FromTryOut<DateTime>(DateTime.TryParse, value)
                        .OrElse(() => Maybe.FromTryOut<long>(long.TryParse, value).Select(x => new DateTime(x)))
                        .GetOrDefault()
                        .ToString("d-MMM-yyyy");
                case FieldTypeEnum.URL:
                {
                    return LabeledUri.TryParse(value, out var uri) ? (object) uri : value;
                }
                default:
                    throw GetInvalidFieldTypeException(type);
            }
        }

        private static string ConvertMoneyToHistoryRecord(decimal value, [CanBeNull] CustomFieldConfig config)
        {
            if (config == null || config.FormatSpecifier == null ||
                config.FormatInfo == null || TpFeature.AllowSpecifyCustomMoneyFormat.IsDisabled())
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            var nfi = (Integration.Messages.Entities.NumberFormatInfo) config.FormatInfo;
            return value.ToString(config.FormatSpecifier, new NumberFormatInfo
            {
                CurrencySymbol = config.Units ?? string.Empty,
                CurrencyDecimalDigits = nfi.CurrencyDecimalDigits,
                CurrencyDecimalSeparator = nfi.CurrencyDecimalSeparator,
                CurrencyGroupSeparator = nfi.CurrencyGroupSeparator,
                CurrencyGroupSizes = nfi.CurrencyGroupSizes
            });
        }

        /// <summary>
        /// Converts boxed number type value to string.
        /// </summary>
        /// <param name="value">Boxed number type value.</param>
        /// <returns>String representation of the value that allows to follow number sort order for returned strings.</returns>
        /// <exception cref="CustomFieldTypeMismatchException">If <paramref name="value"/> is not boxed number type value(int or double for example).</exception>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is negative.</exception>
        [CanBeNull]
        [Pure]
        private static string ConvertBoxedNumberToString(
            [NotNull] object value)
        {
            var sourceType = value.GetType();

            if (sourceType != typeof(byte)
                && sourceType != typeof(sbyte)
                && sourceType != typeof(short)
                && sourceType != typeof(ushort)
                && sourceType != typeof(int)
                && sourceType != typeof(uint)
                && sourceType != typeof(long)
                && sourceType != typeof(ulong)
                && sourceType != typeof(double)
                && sourceType != typeof(decimal))
            {
                throw CustomFieldTypeMismatchException.Create("integer", sourceType.Name);
            }

            if (value is double doubleValue)
            {
                if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
                {
                    return null;
                }
            }

            var sourceString = Convert.ToString(value, CultureInfo.InvariantCulture);
            return sourceString;

        }

        [Pure]
        [NotNull]
        private static Exception GetInvalidFieldTypeException(FieldTypeEnum type)
        {
            return new InvalidOperationException($"Unknown custom field type \"{type}\"");
        }
    }
}
