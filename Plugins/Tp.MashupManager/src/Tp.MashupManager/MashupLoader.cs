// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Linq;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.MashupManager
{
	public class MashupLoader : IMashupLoader
	{
		public Mashup Load(string mashupFolderPath, string mashupName)
		{
			if (!Directory.Exists(mashupFolderPath))
			{
				return null;
			}

			var configFiles = Directory.GetFiles(mashupFolderPath, "*.cfg");
			var mashupConfig = new MashupConfig(configFiles.SelectMany(File.ReadAllLines));
			var files = Directory.GetFiles(mashupFolderPath, "*", SearchOption.AllDirectories).Where(f => !configFiles.Contains(f));
			var mashupFiles = files.Select(f => new MashupFile
			{
				FileName = f.GetRelativePathTo(mashupFolderPath),
				Content = File.ReadAllText(f)
			}).ToList();

			var mashup = new Mashup(mashupFiles)
			{
				Name = mashupName,
				Placeholders = String.Join(",", mashupConfig.Placeholders)
			};

			return mashup;
		}
	}
}