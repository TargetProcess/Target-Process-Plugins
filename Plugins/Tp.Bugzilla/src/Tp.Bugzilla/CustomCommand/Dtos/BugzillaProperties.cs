// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Tp.Bugzilla.Schemas;

namespace Tp.Bugzilla.CustomCommand.Dtos
{
    [DataContract]
    public class BugzillaProperties
    {
        static IEnumerable<string> CustomMappingSupportedVersions => new[]
        {
            "5.0",
            "5.1"
        };

        public BugzillaProperties()
        {
        }

        public BugzillaProperties(bugzilla_properties bugzillaProperties)
        {
            Statuses = bugzillaProperties.statuses.nameCollection.Cast<string>().ToList();
            Priorities = bugzillaProperties.priorities.nameCollection.Cast<string>().ToList();
            Severities = bugzillaProperties.severities.nameCollection.Cast<string>().ToList();
            CustomFields = CustomMappingSupportedVersions.Any(bugzillaProperties.version.StartsWith)
                ? (from custom_field cf in bugzillaProperties.custom_fieldCollection select cf.cf_name).ToList()
                : new List<string>();
        }

        [DataMember]
        public List<string> Statuses { get; set; }

        [DataMember]
        public List<string> Priorities { get; set; }

        [DataMember]
        public List<string> Severities { get; set; }

        [DataMember]
        public List<string> CustomFields { get; set; }
    }
}
