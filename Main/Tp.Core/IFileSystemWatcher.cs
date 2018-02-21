using System;
using System.IO;

namespace Tp.Core
{
    public interface IFileSystemWatcher
    {
        IObservable<FileSystemEventArgs> Watch(string path);
    }
}
