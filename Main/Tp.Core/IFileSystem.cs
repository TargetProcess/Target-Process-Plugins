// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Core
{
	public interface IFileSystem
	{
		bool DirectoryExists(string physicalPath);
		void CreateDirectory(string physicalPath);
		bool FileExists(string physicalPath);
		bool IsDirectory(string physicalPath);
		void Copy(string sourcePath, string destinationPath, bool overwrite);
		string GetFullPath(string relativePath);
		IFileSystemWatcher FileSystemWatcher { get; }
	}
}
