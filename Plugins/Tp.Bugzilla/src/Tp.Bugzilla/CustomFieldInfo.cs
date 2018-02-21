// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Tp.Bugzilla.Schemas;

namespace Tp.Bugzilla
{
    [Serializable]
    [DataContract]
    public class CustomFieldInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public IEnumerable<string> Values { get; set; }

        public CustomFieldInfo()
        {
        }

        public CustomFieldInfo(custom_field source)
        {
            Name = source.cf_name;
            Description = source.cf_description;
            Values = GetBugzillaCustomFieldValues(source);
        }

        public bool HasValue
        {
            get { return Values != null && Values.Any(ValueIsNotEmpty); }
        }

        private static IEnumerable<string> GetBugzillaCustomFieldValues(custom_field bugzillaCustomField)
        {
            return IsCollection(bugzillaCustomField)
                ? GetCustomFieldCollectionValue(bugzillaCustomField)
                : GetCustomFieldValue(bugzillaCustomField);
        }

        private static bool IsCollection(custom_field bugzillaCustomField)
        {
            return "Multiple-Selection Box".Equals(bugzillaCustomField.cf_type, StringComparison.OrdinalIgnoreCase);
        }

        private static bool CustomFieldHasValue(string type, string value)
        {
            if ("Drop Down".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                return ValueIsNotEmpty(value) && !"---".Equals(value, StringComparison.OrdinalIgnoreCase);
            }

            return ValueIsNotEmpty(value);
        }

        private static bool ValueIsNotEmpty(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        private static IEnumerable<string> GetCustomFieldCollectionValue(custom_field field)
        {
            var values = field.cf_values.cf_valueCollection.Cast<string>().Where(ValueIsNotEmpty).ToList();

            return values.Count > 0 ? values : Enumerable.Empty<string>();
        }

        private static IEnumerable<string> GetCustomFieldValue(custom_field bugzillaCustomField)
        {
            return CustomFieldHasValue(bugzillaCustomField.cf_type, bugzillaCustomField.cf_value)
                ? new[] { bugzillaCustomField.cf_value }
                : Enumerable.Empty<string>();
        }
    }
}
