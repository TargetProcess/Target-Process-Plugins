// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tp.Bugzilla.CustomCommand;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.Bugzilla
{
    public interface IBugzillaCustomFieldsMapper
    {
        List<CustomMappingLookup> GetMappableProperties { get; }
        Dictionary<MappingElement, string> GetCusomFildsForBugAddedAction(BugDTO bug, BugzillaProfile profile);
        Dictionary<MappingElement, string> GetCusomFildsForBugUpdatedAction(BugDTO bug, BugField[] changedFields, BugzillaProfile profile);
    }

    public class BugzillaCustomFieldsMapper : IBugzillaCustomFieldsMapper
    {
        private static readonly List<CustomMappingLookup> _mappablePropertyNames;
        private static readonly Dictionary<BugField, PropertyInfo> _mutablePropertyInfos = new Dictionary<BugField, PropertyInfo>();
        private static readonly Dictionary<string, PropertyInfo> _immutablePropertyInfos = new Dictionary<string, PropertyInfo>();

        private static readonly List<string> _excludeFromMapping = new List<string>
        {
            "ID",
            BugField.CommentOnChangingState.ToString(),
            BugField.Description.ToString(),
            BugField.EntityStateID.ToString(),
            BugField.EntityTypeID.ToString(),
            BugField.EntityTypeName.ToString(),
            BugField.LastCommentUserID.ToString(),
            BugField.LastEditorID.ToString(),
            BugField.ResponsibleSquadID.ToString(),
            BugField.Name.ToString(),
            BugField.OwnerID.ToString(),
            BugField.ParentID.ToString(),
            BugField.PriorityID.ToString(),
            BugField.PriorityName.ToString(),
            BugField.ResponsibleSquadID.ToString(),
            BugField.SeverityID.ToString(),
            BugField.SeverityName.ToString(),
            BugField.SquadID.ToString(),
            BugField.SquadIterationID.ToString()
        };

        static BugzillaCustomFieldsMapper()
        {
            var mappablePropertyTypes = new List<Type> { typeof(int), typeof(int?), typeof(long), typeof(long?), typeof(string) };

            var mappablePropertyInfos = typeof(BugDTO).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(
                x =>
                    x.CanRead && x.CanWrite && !_excludeFromMapping.Contains(x.Name) && !x.Name.StartsWith("CustomField")
                    && mappablePropertyTypes.Contains(x.PropertyType)).ToList();

            _mappablePropertyNames = mappablePropertyInfos.Select(mpi => new CustomMappingLookup(mpi.Name)).OrderBy(x => x.Name).ToList();

            foreach (var pi in mappablePropertyInfos)
            {
                BugField bf;
                if (Enum.TryParse(pi.Name, out bf))
                {
                    _mutablePropertyInfos.Add(bf, pi);
                }
                else
                {
                    _immutablePropertyInfos.Add(pi.Name, pi);
                }
            }
        }

        public List<CustomMappingLookup> GetMappableProperties => _mappablePropertyNames;

        public Dictionary<MappingElement, string> GetCusomFildsForBugAddedAction(BugDTO bug, BugzillaProfile profile)
        {
            var result = new Dictionary<MappingElement, string>();

            foreach (var m in profile.CustomMapping.Where(cf => !string.IsNullOrEmpty(cf.Value.Name)))
            {
                var pi =
                    _immutablePropertyInfos.FirstOrDefault(x => string.Compare(x.Key, m.Value.Name, StringComparison.OrdinalIgnoreCase) == 0)
                        .Value;
                if (pi != null)
                {
                    var value = pi.GetValue(bug);
                    result.Add(m, value?.ToString() ?? string.Empty);
                }

                var p =
                    _mutablePropertyInfos.FirstOrDefault(
                        x => string.Compare(x.Value.Name, m.Value.Name, StringComparison.OrdinalIgnoreCase) == 0);
                if (p.Value != null)
                {
                    var value = p.Value.GetValue(bug);
                    result.Add(m, value?.ToString() ?? string.Empty);
                }
            }

            return result;
        }

        public Dictionary<MappingElement, string> GetCusomFildsForBugUpdatedAction(BugDTO bug, BugField[] changedFields,
            BugzillaProfile profile)
        {
            var result = new Dictionary<MappingElement, string>();

            var changedMutables = (from pi in _mutablePropertyInfos where changedFields.Contains(pi.Key) select pi.Value).ToList();

            foreach (var m in profile.CustomMapping.Where(cf => !string.IsNullOrEmpty(cf.Value.Name)))
            {
                var pi = changedMutables.FirstOrDefault(x => string.Compare(x.Name, m.Value.Name, StringComparison.OrdinalIgnoreCase) == 0);
                if (pi != null)
                {
                    var value = pi.GetValue(bug);
                    result.Add(m, value?.ToString() ?? string.Empty);
                }
            }

            return result;
        }
    }
}
