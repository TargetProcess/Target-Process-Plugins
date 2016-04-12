// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;
using StructureMap;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[Profile, Serializable, DataContract]
	public class SampleProfileSerialized : ISynchronizableProfile, IValidatable
	{
		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public int SynchronizationInterval { get; set; }

		public void Validate(PluginProfileErrorCollection errors)
		{
			var logger = ObjectFactory.TryGetInstance<IActivityLogger>();
			if (logger != null)
			{
				logger.Info("validation is in progress");
			}
		}
	}
}