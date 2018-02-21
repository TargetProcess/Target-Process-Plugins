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
