// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;

namespace Tp.Integration.Messages.PluginLifecycle
{
	[Serializable, DataContract]
	public class ProfileErrorCheckResult
	{
		[DataMember]
		public string ProfileName { get; set; }

		[DataMember]
		public bool ErrorsExist { get; set; }
	}
}