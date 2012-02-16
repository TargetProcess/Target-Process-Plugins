// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;

namespace Tp.SourceControl.Diff
{
	public class DiffChunk
	{
		public DiffChunk()
		{
			Lines = new List<string>();
		}

		public void AddLine(string line)
		{
			Lines.Add(line);
		}

		public List<string> Lines { get; private set; }

		public int FirstBegin { get; set; }

		public int FirstEnd { get; set; }

		public int SecondBegin { get; set; }

		public int SecondEnd { get; set; }
	}
}