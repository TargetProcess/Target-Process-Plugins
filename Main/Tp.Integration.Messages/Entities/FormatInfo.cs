using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.Serialization;

namespace Tp.Integration.Messages.Entities
{
    [DataContract]
    [Serializable]
    [KnownType(typeof(NumberFormatInfo))]
    public abstract class FormatInfo : IEquatable<FormatInfo>
    {
        private static readonly Type[] _knownTypes = typeof(FormatInfo).GetCustomAttributes<KnownTypeAttribute>()
            .Select(a => a.Type).ToArray();

        public static Type[] KnownTypes => _knownTypes;

        public abstract bool Equals(FormatInfo other);

        public override bool Equals(object other) => Equals(other as FormatInfo);

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(FormatInfo left, FormatInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FormatInfo left, FormatInfo right)
        {
            return !Equals(left, right);
        }
    }

    [DataContract]
    [Serializable]
    public sealed class NumberFormatInfo : FormatInfo
    {
        [DataMember]
        public int CurrencyDecimalDigits { get; set; }

        [DataMember]
        public string CurrencyDecimalSeparator { get; set; }

        [DataMember]
        public string CurrencyGroupSeparator { get; set; }

        [DataMember]
        public int[] CurrencyGroupSizes { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CurrencyDecimalDigits.GetHashCode();
                hashCode = (hashCode * 397) ^ CurrencyDecimalSeparator.GetHashCode();
                hashCode = (hashCode * 397) ^ CurrencyGroupSeparator.GetHashCode();
                hashCode = (hashCode * 397) ^ (CurrencyGroupSizes?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override bool Equals(FormatInfo other) => Equals(other as NumberFormatInfo);

        public override bool Equals(object other) => Equals(other as NumberFormatInfo);

        private bool Equals(NumberFormatInfo other)
        {
            return other != null
                && CurrencyDecimalDigits == other.CurrencyDecimalDigits
                && CurrencyDecimalSeparator == other.CurrencyDecimalSeparator
                && CurrencyGroupSeparator == other.CurrencyGroupSeparator
                && (ReferenceEquals(CurrencyGroupSizes, other.CurrencyGroupSizes) ||
                    CurrencyGroupSizes != null && other.CurrencyGroupSizes != null &&
                    CurrencyGroupSizes.SequenceEqual(other.CurrencyGroupSizes));
        }

        public override string ToString()
        {
            return $"Currency: {{DecimalDigits: {CurrencyDecimalDigits}, DecimalSeparator: {CurrencyDecimalSeparator},"
                + $" GroupSeparator: {CurrencyGroupSeparator}, GroupSizes: [{string.Join(",", CurrencyGroupSizes)}]}}";
        }
    }

    public class CustomFieldConfigTypeProvider : ITypeProvider
    {
        // Allow to use casts like config.formatinfo.As<NumberFormatInfo>.currencyDecimalDigits in api/v2 calls.
        public IEnumerable<KeyValuePair<string, Type>> GetKnownTypes() =>
            FormatInfo.KnownTypes
                .Select(x => new KeyValuePair<string, Type>(x.Name, x));
    }
}
