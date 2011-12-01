// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.LegacyProfileConvertsion.Common
{
	public interface IConvertorArgs : IDatabaseConfiguration
	{
		string AccountName { get; }
		string TpConnectionString { get; }
		string PluginConnectionString { get; }
	}
}