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
		private readonly IMashupDirectoryIgnoreStrategy _directoryIgnoreStrategy; 

		public MashupCollection(string mashupsPhysicalPath, IMashupDirectoryIgnoreStrategy directoryIgnoreStrategy = null)
		{
			_mashupsPhysicalPath = mashupsPhysicalPath;
			_directoryIgnoreStrategy = directoryIgnoreStrategy ?? new IncludeAllMashupDirectoryIgnoreStrategy();
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

		private IEnumerable<Mashup> ScanForMashups(string mashupsPhysicalPath)
		{
			if (!Directory.Exists(mashupsPhysicalPath))
			{
				return new Mashup[] {};
			}

			IEnumerable<string> directories = Directory.GetDirectories(mashupsPhysicalPath);

			return (from directory in directories
					where !ShouldIgnoreMashupDirectory(directory)
			        let baseDir = directory
			        let configs = Directory.GetFiles(directory, "*.cfg")
			        let mashupConfig = new MashupConfig(configs.SelectMany(File.ReadAllLines))
			        let mashupName = new DirectoryInfo(directory).Name
					let files = GetMashupFiles(directory)
			        select new Mashup
				        {
					        MashupFilePaths = files.Select(x => MakePathRelative(x, baseDir)).ToArray(),
							MashupPhysicalFilePaths = files.ToArray(),
					        MashupName = mashupName,
					        MashupConfig = mashupConfig
				        }).ToList();
		}

		private string[] GetMashupFiles(string directory)
		{
			if (ShouldIgnoreMashupDirectory(directory))
			{
				return new string[0];
			}

			var files = new List<string>();
			files.AddRange(Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly));
			files.AddRange(Directory.GetDirectories(directory).SelectMany(GetMashupFiles));
			return files.ToArray();
		}

		private static string MakePathRelative(string path, string basePath)
		{
			return path.Replace(basePath, ".");
		}
		
		private bool ShouldIgnoreMashupDirectory(string directory)
		{
			return _directoryIgnoreStrategy.ShouldIgnoreMashupDirectory(directory);
		}
	}
}