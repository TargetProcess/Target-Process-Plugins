// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;

namespace Tp.Core
{
	public class LocalFileSystem : IFileSystem
	{
		private readonly IFileSystemWatcher _fileSystemWatcher;
		public LocalFileSystem()
		{
			_fileSystemWatcher = new FileSystemWatcherImpl();
		}

		public virtual bool DirectoryExists(string physicalPath)
		{
			return Directory.Exists(physicalPath);
		}

		public virtual void CreateDirectory(string physicalPath)
		{
			Directory.CreateDirectory(physicalPath);
		}

		public virtual bool FileExists(string physicalPath)
		{
			return File.Exists(physicalPath);
		}

		public virtual bool IsDirectory(string physicalPath)
		{
			// Throws an exception if path is inaccessible. This is definitely what we need.
			// Does not throw exception for nonexistant local paths.
			return new FileInfo(physicalPath).Attributes.HasFlag(FileAttributes.Directory);
		}

		public virtual void Copy(string sourcePath, string destinationPath, bool overwrite)
		{
			File.Copy(sourcePath, destinationPath, overwrite);
		}

		public virtual string GetFullPath(string relativePath)
		{
			return Path.GetFullPath(relativePath);
		}

		public virtual IFileSystemWatcher FileSystemWatcher
		{
			get { return _fileSystemWatcher; }
		}
	}
}
