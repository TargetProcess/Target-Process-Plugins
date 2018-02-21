using System;
using System.IO;
using System.Reactive.Linq;

namespace Tp.Core
{
    internal class FileSystemWatcherImpl : IFileSystemWatcher
    {
        public IObservable<FileSystemEventArgs> Watch(string path)
        {
            return Observable.Create<FileSystemEventArgs>(o =>
            {
                var watcher = new FileSystemWatcher
                {
                    Path = path,
                    Filter = string.Empty,
                    IncludeSubdirectories = true
                };
                var changed = Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(h => (_, args) => h(args),
                    h => watcher.Changed += h,
                    h => watcher.Changed -= h);
                var created = Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(h => (_, args) => h(args),
                    h => watcher.Created += h,
                    h => watcher.Created -= h);
                var deleted = Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(h => (_, args) => h(args),
                    h => watcher.Deleted += h,
                    h => watcher.Deleted -= h);
                var renamed = Observable.FromEvent<RenamedEventHandler, FileSystemEventArgs>(h => (_, args) => h(args),
                    h => watcher.Renamed += h,
                    h => watcher.Renamed -= h);
                var source = changed.Merge(created).Merge(deleted).Merge(renamed);
                var token = source.Subscribe(o);
                watcher.EnableRaisingEvents = true;
                return new CompositeDisposable(new[] { Disposable.Create(watcher.Dispose), token });
            });
        }
    }
}
