// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.Bugzilla.CustomCommand.Dtos
{
	[DataContract]
	public class TargetProcessInfo
	{
		[DataMember]
		public MappingContainer States { get; set; }
	}
}