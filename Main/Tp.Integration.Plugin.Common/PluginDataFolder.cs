// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using Tp.Integration.Plugin.Common.Properties;

namespace Tp.Integration.Plugin.Common
{
	/// <summary>
	/// Folder of Plugin data. Can be used to store some plugin specific information.
	/// </summary>
	public class PluginDataFolder
	{
		/// <summary>
		/// Path to Plugin data folder.
		/// </summary>
		public string Path
		{
			get
			{
				var path = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.PluginDataFolder));
				
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				return path; 
			}
		}
	}
}