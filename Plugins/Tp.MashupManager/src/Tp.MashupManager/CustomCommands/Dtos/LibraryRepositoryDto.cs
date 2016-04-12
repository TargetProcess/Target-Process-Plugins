// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.MashupManager.MashupLibrary.Package;

namespace Tp.MashupManager.CustomCommands.Dtos
{
	public class LibraryRepositoryDto
	{
		public string Name { get; set; }
		public IEnumerable<LibraryPackage> Packages { get; set; }
	}
}