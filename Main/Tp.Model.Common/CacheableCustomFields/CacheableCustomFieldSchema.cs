using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;

namespace Tp.Model.Common.CacheableCustomFields
{
    /// <summary>
    /// See `adr/2020-02-24 - Version-based in-memory caching.md` for details.
    /// </summary>
    public class CacheableCustomFieldSchema
    {
        public static readonly CacheableCustomFieldSchema Empty = new CacheableCustomFieldSchema(
            Array.Empty<CacheableCustomField>().ToReadOnlyCollection(), -1L);

        public CacheableCustomFieldSchema(
            [NotNull] IReadOnlyList<CacheableCustomField> fields,
            long version)
        {
            Fields = Argument.NotNull(nameof(fields), fields);
            Version = version;
        }

        public IReadOnlyList<CacheableCustomField> Fields { get; }

        /// <summary>
        /// Automatically incremented counter by database trigger.
        /// </summary>
        public long Version { get; }
    }

    /// <summary>
    /// Immutable custom field information which can be safely cached in-memory
    /// </summary>
    public class CacheableCustomField : ICustomFieldInfo
    {
        public CacheableCustomField(
            int id,
            string name,
            FieldTypeEnum fieldType,
            int entityTypeId,
            string entityFieldName,
            int? processId,
            string description = "",
            bool required = false,
            bool isSystem = false,
            bool enabledForFilter = false,
            string value = "",
            double numericPriority = 0.0,
            CacheableCustomFieldConfig config = null)
        {
            Id = id;
            Name = name;
            Description = description;
            FieldType = fieldType;
            Required = required;
            IsSystem = isSystem;
            EnabledForFilter = enabledForFilter;
            EntityFieldName = entityFieldName;
            Value = value;
            ProcessId = processId;
            EntityTypeId = entityTypeId;
            NumericPriority = numericPriority;
            Config = config ?? CacheableCustomFieldConfig.Empty;
        }

        public int Id { get; }

        public string Name { get; }

        public string Description { get; }

        public FieldTypeEnum FieldType { get; }

        public bool Required { get; }

        public bool IsSystem { get; }

        public bool EnabledForFilter { get; }

        public string EntityFieldName { get; }

        public string Value { get; }

        public int? ProcessId { get; }

        public int EntityTypeId { get; }

        int? ICustomFieldInfo.EntityTypeID => EntityTypeId;

        public double NumericPriority { get; }

        public CacheableCustomFieldConfig Config { get; }

        string ICustomFieldConfigHolder.Units => Config.Units;

        string IFormattableCustomFieldConfig.FormatSpecifier => Config.FormatSpecifier;

        FormatInfo IFormattableCustomFieldConfig.FormatInfo => Config.FormatInfo;

        string ICustomFieldConfigHolder.CalculationModel => Config.CalculationModel;

        public bool IsCalculated()
        {
            ICustomFieldInfo t = this;
            return t.GetIsCalculated();
        }
    }

    public class CacheableCustomFieldConfig
    {
        public static readonly CacheableCustomFieldConfig Empty = new CacheableCustomFieldConfig("", false, "", null, null, null);

        public CacheableCustomFieldConfig(
            string calculationModel,
            bool calculationModelContainsCollections,
            string units,
            string defaultValue,
            string formatSpecifier,
            FormatInfo formatInfo)
        {
            CalculationModel = calculationModel;
            CalculationModelContainsCollections = calculationModelContainsCollections;
            Units = units;
            DefaultValue = defaultValue;
            FormatSpecifier = formatSpecifier;
            FormatInfo = formatInfo;
        }

        [CanBeNull]
        public string CalculationModel { get; }

        public bool CalculationModelContainsCollections { get; }

        [CanBeNull]
        public string Units { get; }

        [CanBeNull]
        public string DefaultValue { get; }

        [CanBeNull]
        public string FormatSpecifier { get; }

        [CanBeNull]
        public FormatInfo FormatInfo { get; }
    }
}
