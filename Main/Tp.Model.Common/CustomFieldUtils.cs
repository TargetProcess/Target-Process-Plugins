using System;
using System.Collections.Generic;
using System.Linq;
using Tp.BusinessObjects;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;

namespace Tp.Model.Common
{
    public static class CustomFieldUtils
    {
        public const string ValuesSeparator = "\n";

        public static string ToRawValue(IEnumerable<string> value)
        {
            return string.Join(ValuesSeparator, value);
        }

        public static IEnumerable<string> FromRawValue(string value)
        {
            return value.IsBlank()
                ? Array.Empty<string>()
                : value.Split(new[] { ValuesSeparator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.TrimToEmpty());
        }

        public static object ParseCustomFieldValue(string value, FieldTypeEnum type, IFormattableCustomFieldConfig config)
        {
            return CustomFieldValueConverter.ConvertStringToValueObject(value, type, config);
        }
    }
}
