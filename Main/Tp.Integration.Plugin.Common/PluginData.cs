// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Plugin.Common
{
	public class PluginData
	{
		internal PluginData(string name, string description, string category, string iconFilePath)
		{
			Name = name;
			Description = description;
			Category = category;
			IconFilePath = iconFilePath;
		}

		/// <summary>
		/// The plugin Name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The plugin description.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// The plugin category.
		/// </summary>
		public string Category { get; private set; }

		/// <summary>
		/// Relative Path to plugin icon file.
		/// </summary>
		public string IconFilePath { get; private set; }
	}
}