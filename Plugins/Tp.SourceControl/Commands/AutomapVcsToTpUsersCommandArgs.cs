// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Runtime.Serialization;
using Tp.SourceControl.Settings;

namespace Tp.SourceControl.Commands
{
	[DataContract]
	public class AutomapVcsToTpUsersCommandArgs
	{
		public AutomapVcsToTpUsersCommandArgs()
		{
			TpUsers = new List<TpUserMappingInfo>();
			Connection = new ConnectionSettings();
		}

		[DataMember]
		public List<TpUserMappingInfo> TpUsers { get; set; }

		[DataMember]
		public ConnectionSettings Connection { get; set; }
	}
}