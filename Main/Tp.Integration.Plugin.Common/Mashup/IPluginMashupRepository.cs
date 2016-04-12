// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Plugin.Common.Mashup
{
	/// <summary>
	/// Represents a collection of mashups for plugin. To add mashups for your inherit from this class.
	/// </summary>
	public interface IPluginMashupRepository
	{
		/// <summary>
		/// Mashups provided by plugin
		/// </summary>
		PluginMashup[] PluginMashups { get; }
	}
}