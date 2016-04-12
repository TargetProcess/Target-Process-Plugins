using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tp.MashupManager
{
	public interface IMashupLoader
	{
		Mashup Load(string mashupFolderPath, string mashupName);
	}
}
