using System;
using System.IO;

namespace Tp.Core
{
	public interface IFileSystemWatcher
	{
		IDisposable Subscribe(string path, Action<FileSystemEventArgs> handler);
	}
}