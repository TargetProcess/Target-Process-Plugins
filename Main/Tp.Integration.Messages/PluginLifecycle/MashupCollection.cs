// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tp.Integration.Messages.PluginLifecycle
{
	public class MashupCollection : IEnumerable<Mashup>
	{
		private readonly string _mashupsPhysicalPath;
		private IEnumerable<Mashup> _mashups;

		public MashupCollection(string mashupsPhysicalPath)
		{
			_mashupsPhysicalPath = mashupsPhysicalPath;
		}

		private static IEnumerable<Mashup> ScanForMashups(string mashupsPhysicalPath)
		{
			if (!Directory.Exists(mashupsPhysicalPath))
			{
				return new Mashup[] { };
			}

			var mashups = new List<Mashup>();
			var directories = GetMashupsDirectories(mashupsPhysicalPath);

			foreach (var directory in directories)
			{
				var baseDir = directory;
				var configs = Directory.GetFiles(directory, "*.cfg");
				var mashupConfig = new MashupConfig(configs.SelectMany(File.ReadAllLines));

				var mashupName = new DirectoryInfo(directory).Name;
				var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
				
				mashups.Add(new Mashup
					{
						MashupFilePaths = files.Select(x => MakePathRelative(x, baseDir)).ToArray(),
						MashupName = mashupName,
						MashupConfig = mashupConfig
					});
			}

			return mashups;
		}

		private static IEnumerable<string> GetMashupsDirectories(string mashupsPhysicalPath)
		{
			return Directory.GetDirectories(mashupsPhysicalPath);
		} 

		private static string MakePathRelative(string path, string basePath)
		{
			return path.Replace(basePath, ".");
		}

		public IEnumerator<Mashup> GetEnumerator()
		{
			if (_mashups == null)
			{
				_mashups = ScanForMashups(_mashupsPhysicalPath);
			}
			return _mashups.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}