using System;
using System.IO;

namespace Tp.Core
{
	internal class FileSystemWatcherImpl : IFileSystemWatcher
	{
		public IDisposable Subscribe(string path, Action<FileSystemEventArgs> handler)
		{
			var watcher = new FileSystemWatcher
				{
					Path = path,
					Filter = string.Empty,
					IncludeSubdirectories = true
				};
			FileSystemEventHandler h1 = (_, args) => handler(args);
			RenamedEventHandler h2 = (_, args) => handler(args);
			watcher.Changed += h1;
			watcher.Created += h1;
			watcher.Deleted += h1;
			watcher.Renamed += h2;
			watcher.EnableRaisingEvents = true;
			return Disposable.Create(() =>
				{
					watcher.EnableRaisingEvents = false;
					watcher.Changed -= h1;
					watcher.Created -= h1;
					watcher.Deleted -= h1;
					watcher.Renamed -= h2;
				});
		}
	}
}