using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.Entities
{
    /// <summary>
    /// Custom field.
    /// </summary>
    [Serializable]
    [DataContract]
    public class Field : ICustomFieldInfo
    {
        /// <summary>
        /// Field name as presented on the user interface.
        /// </summary>
        [DataMember]
        [XmlElement(Order = 10)]
        public string Name { get; set; }

        /// <summary>
        /// System field name, like CustomField1, CustomField1, etc.
        /// </summary>
        [DataMember]
        [XmlElement(Order = 20)]
        public string EntityFieldName { get; set; }

        /// <summary>
        /// Field type.
        /// </summary>
        [DataMember]
        [XmlElement(Order = 30)]
        public FieldTypeEnum FieldType { get; set; }

        /// <summary>
        /// Whether this is a required field.
        /// </summary>
        [DataMember]
        [XmlElement(Order = 40)]
        public bool Required { get; set; }

        /// <summary>
        /// Unparsed field value.
        /// </summary>
        [DataMember]
        [XmlElement(Order = 50)]
        public string Value { get; set; }

        [DataMember]
        [XmlElement(Order = 60)]
        public List<string> Items { get; set; }

        [DataMember]
        [XmlElement(Order = 70)]
        public string DefaultValue { get; set; }

        [DataMember]
        [XmlElement(Order = 71)]
        public string Description { get; set; }

        [DataMember]
        [XmlElement(Order = 80)]
        public string CalculationModel { get; set; }

        [DataMember]
        [XmlElement(Order = 81)]
        public string Units { get; set; }

        [XmlElement(Order = 90)]
        public int? EntityTypeID { get; set; }

        [DataMember]
        [XmlElement(Order = 91)]
        public bool? CalculationModelContainsCollections { get; set; }

        /// <summary>
        /// Whether this is a system field.
        /// </summary>
        [DataMember]
        [XmlElement(Order = 100)]
        public bool IsSystem { get; set; }

        public Field()
        {
            FieldType = FieldTypeEnum.None;
        }

        public Field(Field other)
        {
            Name = other.Name;
            EntityFieldName = other.EntityFieldName;
            FieldType = other.FieldType;
            Required = other.Required;
            Value = other.Value;
            DefaultValue = other.DefaultValue;
            CalculationModel = other.CalculationModel;
            Units = other.Units;
            CalculationModelContainsCollections = other.CalculationModelContainsCollections;
            IsSystem = other.IsSystem;
            Items = new List<string>(other.Items);
        }

        // To share parsing logic between TP and plugins.
        public string[] ParseMultipleSelectionListFieldValue()
        {
            if (FieldType == FieldTypeEnum.MultipleSelectionList)
            {
                return string.IsNullOrEmpty(Value) ? Array.Empty<string>() : Value.Split(',').Select(x => x.Trim()).ToArray();
            }
            return Array.Empty<string>();
        }

        public override string ToString()
        {
            long ticks;

            if (FieldType == FieldTypeEnum.Date && Int64.TryParse(Value, out ticks))
            {
                return new DateTime(ticks).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            }

            double number;
            if (FieldType == FieldTypeEnum.Number && double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
            {
                return number.ToString(CultureInfo.InvariantCulture);
            }

            return Value;
        }

        public bool Equals(Field other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name)
                && Equals(other.FieldType, FieldType)
                && Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Field)) return false;
            return Equals((Field) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result * 397) ^ FieldType.GetHashCode();
                result = (result * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                return result;
            }
        }
    }
}
