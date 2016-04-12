using System.Collections.Generic;

namespace Tp.Search.Model.Document
{
	interface IFileService
	{
		IEnumerable<string> GetFiles(string path, string pattern);
	}
}