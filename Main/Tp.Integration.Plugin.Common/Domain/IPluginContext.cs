// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
	/// <summary>
	/// Provides access for context in which current message is handling. Almost all classes from API depents on context values.
	/// Injected into StructureMap container.
	/// </summary>
	public interface IPluginContext
	{
		AccountName AccountName { get; }
		ProfileName ProfileName { get; }
		PluginName PluginName { get; }
	}
}