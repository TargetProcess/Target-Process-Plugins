// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.PluginLifecycle;

namespace Tp.Integration.Plugin.Common.Mashup
{
	/// <summary>
	/// Represents a plugin mashup.
	/// </summary>
	public class PluginMashup
	{
		/// <summary>
		/// Creates a new instance of plugin mashup.
		/// </summary>
		/// <param name="mashupName">The name of the mashup. This value is used for mashup folder name at TargetProcess side.</param>
		/// <param name="filePaths">The collection of paths to files to be included in mashup. Paths may be relative or absolute.</param>
		/// <param name="placeholders">The collection of TargetProcess placeholders for which this mashup will be included.</param>
		public PluginMashup(string mashupName, IEnumerable<string> filePaths, string[] placeholders)
		{
			MashupName = mashupName;
            _filePaths = filePaths;
			Placeholders = placeholders;
		}

		public string MashupName { get; set; }
		
		public string[] Placeholders { get; private set; }

		public virtual bool IsValid
		{
            get
            {
				return _filePaths.Select(GetFullPath).All(File.Exists);
            }
		}

		private readonly IEnumerable<string> _filePaths;

		public virtual PluginMashupScript[] GetScripts()
		{
			return
                _filePaths.Select(
                    x => new PluginMashupScript { FileName = x, ScriptContent = Convert.ToBase64String(File.ReadAllBytes(GetFullPath(x))) }).
					ToArray();
		}

		protected bool HasExplicitConfig
		{
			get
			{
				return _filePaths.Any(x => x.Trim().ToLower().EndsWith(".cfg"));
			}
		}

	    private string GetFullPath(string path)
	    {
	        var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}\\{1}", DefaultPluginMashupRepository.PluginMashupDefaultPath, MashupName));
            return Path.Combine(basePath, path);
	    }
	}
}