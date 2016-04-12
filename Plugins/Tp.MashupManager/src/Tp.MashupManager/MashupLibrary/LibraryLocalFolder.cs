// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Plugin.Common;

namespace Tp.MashupManager.MashupLibrary
{
	public class LibraryLocalFolder : ILibraryLocalFolder
	{
		private const string MashupLibraryFolderName = "MashupManagerLibrary";

		public LibraryLocalFolder(PluginDataFolder pluginDataFolder)
		{
			Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginDataFolder.Path, MashupLibraryFolderName);
		}

		public string Path { get; private set; }
	}
}