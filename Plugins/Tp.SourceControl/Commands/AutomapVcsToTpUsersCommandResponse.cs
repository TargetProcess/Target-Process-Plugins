// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.SourceControl.Commands
{
	[DataContract]
	public class AutomapVcsToTpUsersCommandResponse
	{
		public AutomapVcsToTpUsersCommandResponse()
		{
			UserLookups = new Dictionary<string, MappingLookup>();
			Comment = string.Empty;
		}

		[DataMember]
		public Dictionary<string, MappingLookup> UserLookups { get; set; }

		[DataMember]
		public string Comment { get; set; }
	}
}