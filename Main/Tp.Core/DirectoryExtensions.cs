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

        public static bool HasParentChildRelation(this DirectoryInfo expectedParent, DirectoryInfo maybeOrphan)
        {
            for (var current = maybeOrphan; current != null; current = current.Parent)
            {
                if (expectedParent.FullName.Equals(current.FullName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
