// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Runtime.Serialization;

namespace Tp.Integration.Plugin.TestRunImport.Mappers
{
	[Serializable, DataContract]
	public class NamesMapper
	{
		public NamesMapper(string @case, string test)
		{
			Case = @case;
			Test = test;
		}

		[DataMember(Name = "Case")]
		public string Case { get; set; }

		[DataMember(Name = "Test")]
		public string Test { get; set; }
	}
}