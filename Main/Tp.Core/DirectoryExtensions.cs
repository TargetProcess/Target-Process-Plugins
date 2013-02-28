// ReSharper disable CheckNamespace
namespace System.IO
// ReSharper restore CheckNamespace
{
	public static class DirectoryExtensions
	{
		public static void DeleteDirectory(this string path)
		{
			if (!Directory.Exists(path))
				return;


			string[] files = Directory.GetFiles(path);
			string[] dirs = Directory.GetDirectories(path);

			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}

			Directory.Delete(path, false);
		}
	}
}