// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.SourceControl.Diff
{
	public class DiffProcessor : IDiffProcessor
	{
		public DiffResult GetDiff(string baseSources, string diffSource)
		{
			var result = new DiffResult();
			var diff = Diff.DiffText(baseSources, diffSource, true, true, false);
			baseSources = baseSources.Replace("\r", "");
			diffSource = diffSource.Replace("\r", "");
			var aLines = baseSources.Split('\n');
			var bLines = diffSource.Split('\n');

			var n = 0;
			var leftLineNumber = 0;
			var rightLineNumber = 0;

			for (var diffItem = 0; diffItem < diff.Length; diffItem++)
			{
				var aItem = diff[diffItem];

				// write unchanged lines
				while ((n < aItem.StartB) && (n < bLines.Length))
				{
					result.LeftPan.Add(new DiffLineData(bLines[n], DiffActionType.None, leftLineNumber));
					result.RightPan.Add(new DiffLineData(bLines[n], DiffActionType.None, rightLineNumber));
					n++;
					leftLineNumber++;
					rightLineNumber++;
				} // while


				for (var m = 0; m < aItem.deletedA; m++)
				{
					result.LeftPan.Add(new DiffLineData(aLines[aItem.StartA + m], DiffActionType.Deleted, leftLineNumber));
					result.RightPan.Add(new DiffLineData("", DiffActionType.None, -1));
					leftLineNumber++;
				} // for

				// write inserted lines
				while (n < aItem.StartB + aItem.insertedB)
				{
					result.RightPan.Add(new DiffLineData(bLines[n], DiffActionType.Added, rightLineNumber));
					result.LeftPan.Add(new DiffLineData("", DiffActionType.None, -1));
					n++;
					rightLineNumber++;
				}
			}

			// write rest of unchanged lines
			while (n < bLines.Length)
			{
				result.LeftPan.Add(new DiffLineData(bLines[n], DiffActionType.None, leftLineNumber));
				result.RightPan.Add(new DiffLineData(bLines[n], DiffActionType.None, rightLineNumber));
				leftLineNumber++;
				rightLineNumber++;
				n++;
			}

			PostProcessPans(result);

			return result;
		}

		private static void PostProcessPans(DiffResult result)
		{
			var i = 0;

			while (i < result.LeftPan.Count)
			{
				var removedLines = CountOfLinese(result, DiffActionType.Deleted, i);
				if (removedLines > 0)
				{
					var addedLines = CountOfLinese(result, DiffActionType.Added, i + removedLines);
					var offset = 0;
					var count = removedLines;
					if (removedLines > addedLines)
					{
						offset = removedLines - addedLines;
						count = addedLines;
					}
					if (count == 0)
					{
						i += Math.Max(removedLines, addedLines);
					}
					else
					{
						MakeLinesUpdated(result, i + offset, count*2);
						i += count;
					}
				}
				else
				{
					i++;
				}
			}
		}

		private static void MakeLinesUpdated(DiffResult result, int position, int count)
		{
			var leftPos = position;
			var rightPos = position;

			for (var i = 0; i < count; i++)
			{
				if (result.LeftPan[leftPos].Action == DiffActionType.Deleted)
				{
					result.LeftPan[leftPos].Action = DiffActionType.Updated;
					result.RightPan.RemoveAt(rightPos);
					leftPos++;
					continue;
				}

				if (result.RightPan[rightPos].Action == DiffActionType.Added)
				{
					result.RightPan[rightPos].Action = DiffActionType.Updated;
					result.LeftPan.RemoveAt(leftPos);
					rightPos++;
					continue;
				}
			}
		}

		private static int CountOfLinese(DiffResult result, DiffActionType action, int i)
		{
			var count = 0;
			while (i < result.LeftPan.Count)
			{
				if (result.LeftPan[i].Action == action || result.RightPan[i].Action == action)
					count++;
				else
					break;
				i++;
			}
			return count;
		}
	}
}