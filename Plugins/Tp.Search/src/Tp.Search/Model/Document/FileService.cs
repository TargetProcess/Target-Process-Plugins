using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tp.Search.Model.Document
{
	class FileService : IFileService
	{
		public IEnumerable<string> GetFiles(string path, string pattern)
		{
			return Directory.Exists(path) ? Directory.GetFiles(path, pattern) : Enumerable.Empty<string>();
		}
	}
}