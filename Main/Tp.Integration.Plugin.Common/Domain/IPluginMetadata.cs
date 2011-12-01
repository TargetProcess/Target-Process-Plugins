// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Integration.Plugin.Common.Domain
{
	public interface IPluginMetadata
	{
		PluginData PluginData { get; }
		Type ProfileType { get; }
		bool IsSyncronizableProfile { get; }
		bool IsNewProfileInitializable { get; }
		bool IsUpdatedProfileInitializable { get; }
	}
}