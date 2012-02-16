// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
				return new Mashup[] {};

			var mashups = new List<Mashup>();
			var directories = GetMashupsDirectories(mashupsPhysicalPath);

			foreach (var directory in directories)
			{
				var baseDir = directory;
				var configs = Directory.GetFiles(directory, "*.cfg");
				var mashupConfig = ReadPlaceholdersFromConfig(configs);

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

		private static MashupConfig ReadPlaceholdersFromConfig(string[] filePaths)
		{
			var accounts = new List<string>();
			var placeholders = new List<string>();
			foreach (var path in filePaths)
			{
				foreach (var line in File.ReadAllLines(path))
				{
					accounts.AddRange(ExtractValues(line, MashupConfig.AccountsConfigPrefix));
					placeholders.AddRange(ExtractValues(line, MashupConfig.PlaceholderConfigPrefix));
				}
			}

			return new MashupConfig(placeholders.Select(x => x.Trim().ToLower()).Distinct().ToArray(),
			                        accounts.Select(x => x.Trim().ToLower()).Distinct().Select(x => new AccountName(x)).ToArray());
		}

		private static IEnumerable<string> ExtractValues(string line, string matchString)
		{
			if (line.Contains(matchString))
			{
				var values = line.Replace(matchString, string.Empty);
				return string.IsNullOrEmpty(values) ? new string[] {} : values.Split(',');
			}
			return new string[] {};
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