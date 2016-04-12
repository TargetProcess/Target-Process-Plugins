// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Plugin.Common
{
	/// <summary>
	/// Specifies than assembly provides plugins.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class PluginAssemblyAttribute : Attribute
	{
		/// <summary>
		/// Creates an instance of plugin assembly attribute. Attribute data will be used on UI.
		/// </summary>
		/// <param name="name">The plugin name.</param>
		/// <param name="description">The plugin description</param>
		/// <param name="category">The plugin category.</param>
		/// <param name="iconFilePath">Path to plugin icon</param>
		public PluginAssemblyAttribute(string name, string description, string category, string iconFilePath)
		{
			Name = name;
			Description = description;
			Category = string.IsNullOrEmpty(category) ? "No Category" : category;
			IconFilePath = iconFilePath;
		}

		/// <summary>
		/// Creates an instance of plugin assembly attribute. Attribute data will be used on UI.
		/// </summary>
		/// <param name="name">The plugin name.</param>
		/// <param name="description">The plugin description</param>
		/// <param name="category">The plugin category.</param>
		public PluginAssemblyAttribute(string name, string description, string category) : this(name, description, category, string.Empty)
		{
		}

		/// <summary>
		/// Creates an instance of plugin assembly attribute. Attribute data will be used on UI.
		/// </summary>
		/// <param name="name">The plugin name.</param>
		public PluginAssemblyAttribute(string name)
			: this(name, string.Empty, string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// The plugin Name.
		/// </summary>
		private string Name { get; set; }

		/// <summary>
		/// The plugin description.
		/// </summary>
		private string Description { get; set; }

		/// <summary>
		/// The plugin category.
		/// </summary>
		private string Category { get; set; }

		private string IconFilePath { get; set; }

		public PluginData GetData()
		{
			return new PluginData(Name, Description, Category, IconFilePath);
		}
	}
}