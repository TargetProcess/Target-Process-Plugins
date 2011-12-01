// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[Serializable, DataContract]
	public class TestProfile
	{
		[DataMember]
		public string TestProperty { get; set; }
	}
}