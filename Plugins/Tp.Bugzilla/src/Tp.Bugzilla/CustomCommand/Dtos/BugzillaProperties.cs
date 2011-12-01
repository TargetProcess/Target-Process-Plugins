// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
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
		public BugzillaProperties()
		{
		}

		public BugzillaProperties(bugzilla_properties bugzillaProperties)
		{
			Statuses = bugzillaProperties.statuses.nameCollection.Cast<string>().ToList();
			Priorities = bugzillaProperties.priorities.nameCollection.Cast<string>().ToList();
			Severities = bugzillaProperties.severities.nameCollection.Cast<string>().ToList();
		}

		[DataMember]
		public List<string> Statuses { get; set; }

		[DataMember]
		public List<string> Priorities { get; set; }

		[DataMember]
		public List<string> Severities{ get; set; }
	}
}