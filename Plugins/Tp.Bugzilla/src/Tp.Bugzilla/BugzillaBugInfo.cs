// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Tp.Bugzilla
{
	[Serializable]
	[DataContract]
	public class BugzillaBugInfo
	{
		[DataMember]
		public int? TpId { get; set; }

		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string Reporter { get; set; }

		[DataMember]
		public string OS { get; set; }

		[DataMember]
		public string Component { get; set; }

		[DataMember]
		public string Version { get; set; }

		[DataMember]
		public string Classification { get; set; }

		[DataMember]
		public string Platform { get; set; }

		[DataMember]
		public List<CustomFieldInfo> CustomFields { get; set; }

		[DataMember]
		public string Url { get; set; }

		public BugzillaBugInfo()
		{
		}

		public BugzillaBugInfo(BugzillaBug source)
		{
			Id = source.bug_id;
			Reporter = source.reporter;
			OS = source.op_sys;
			Component = source.component;
			Version = source.version;
			Classification = source.classification;
			Platform = source.rep_platform;
			CustomFields = source.customFields
				.Select(f => new CustomFieldInfo(f))
				.Where(f => f.HasValue)
				.ToList();
		}
	}
}