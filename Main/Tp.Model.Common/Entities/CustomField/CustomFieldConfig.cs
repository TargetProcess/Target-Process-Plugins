using System;
using System.Runtime.Serialization;
using Tp.Integration.Messages.Entities;

namespace Tp.Model.Common.Entities.CustomField
{
    [Serializable]
    [DataContract]
    public class CustomFieldConfig : ICustomFieldConfigHolder, IEquatable<CustomFieldConfig>
    {
        private string _calculationModel = "";

        public bool Equals(CustomFieldConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DefaultValue == other.DefaultValue && CalculationModel == other.CalculationModel
                && CalculationModelContainsCollections == other.CalculationModelContainsCollections
                && Units == other.Units && FormatSpecifier == other.FormatSpecifier && FormatInfo == other.FormatInfo;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CustomFieldConfig) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (DefaultValue != null ? DefaultValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CalculationModel != null ? CalculationModel.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CalculationModelContainsCollections.GetHashCode();
                hashCode = (hashCode * 397) ^ (FormatSpecifier != null ? FormatSpecifier.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FormatInfo != null ? FormatInfo.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(CustomFieldConfig left, CustomFieldConfig right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CustomFieldConfig left, CustomFieldConfig right)
        {
            return !Equals(left, right);
        }

        public CustomFieldConfig()
        {
        }

        public CustomFieldConfig(CustomFieldConfig prototype)
        {
            DefaultValue = prototype.DefaultValue;
            CalculationModel = prototype.CalculationModel;
            CalculationModelContainsCollections = prototype.CalculationModelContainsCollections;
            Units = prototype.Units;
            FormatSpecifier = prototype.FormatSpecifier;
            FormatInfo = prototype.FormatInfo;
        }

        [DataMember]
        public string DefaultValue { get; set; }

        [DataMember]
        public string CalculationModel
        {
            get => _calculationModel;
            set => _calculationModel = value ?? "";
        }

        [DataMember]
        public bool? CalculationModelContainsCollections { get; set; }

        [DataMember]
        public string Units { get; set; }

        [DataMember]
        public string FormatSpecifier { get; set; }

        [DataMember]
        public FormatInfo FormatInfo { get; set; }
    }
}
