// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tp.SourceControl.Diff
{
	public class ChunkParser
	{
		private static readonly Regex BeginChunkPatern =
			new Regex(@"@@\s+[+-](?<g1>\d+),(?<g2>\d+)\s+[+-](?<g3>\d+),(?<g4>\d+)\s+@@");

		public static List<DiffChunk> Parse(string unidiffResult)
		{
			var chunks = new List<DiffChunk>();
			unidiffResult = unidiffResult.Replace("\r", "");
			var lines = unidiffResult.Split(new[] {'\n'});
			DiffChunk currentChunk = null;
			var i = 0;

			while (i < lines.Length)
			{
				var chunkBeginMatches = BeginChunkPatern.Match(lines[i]);
				if (chunkBeginMatches.Success)
				{
					if (currentChunk != null)
						chunks.Add(currentChunk);
					currentChunk = new DiffChunk
					               	{
					               		FirstBegin = Int32.Parse(chunkBeginMatches.Groups["g1"].Value),
					               		FirstEnd = Int32.Parse(chunkBeginMatches.Groups["g2"].Value),
					               		SecondBegin = Int32.Parse(chunkBeginMatches.Groups["g3"].Value),
					               		SecondEnd = Int32.Parse(chunkBeginMatches.Groups["g4"].Value)
					               	};
				}
				else if (currentChunk != null)
				{
					currentChunk.AddLine(lines[i]);
				}
				i++;
			}

			if (currentChunk != null)
				chunks.Add(currentChunk);

			return chunks;
		}
	}
}