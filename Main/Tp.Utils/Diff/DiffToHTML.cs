using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tp.Components.Diff
{
	public class DiffChunk
	{
		public List<string> lines = new List<string>();
		public int firstBegin;
		public int firstEnd;
		public int secondBegin;
		public int secondEnd;

		public void AddLine(string line)
		{
			lines.Add(line);
		}

		public List<string> Lines
		{
			get { return lines; }
		}

		public int FirstBegin
		{
			get { return firstBegin; }
			set { firstBegin = value; }
		}

		public int FirstEnd
		{
			get { return firstEnd; }
			set { firstEnd = value; }
		}

		public int SecondBegin
		{
			get { return secondBegin; }
			set { secondBegin = value; }
		}

		public int SecondEnd
		{
			get { return secondEnd; }
			set { secondEnd = value; }
		}
	}

	public class ChunkParser
	{
		static readonly Regex beginChunkPatern = new Regex(@"@@\s+[+-](?<g1>\d+),(?<g2>\d+)\s+[+-](?<g3>\d+),(?<g4>\d+)\s+@@");

		public static List<DiffChunk> Parse(string unidiffResult)
		{
			List<DiffChunk> chunks = new List<DiffChunk>();
			unidiffResult = unidiffResult.Replace("\r", "");
			string[] lines = unidiffResult.Split(new char[] { '\n' });
			DiffChunk currentChunk = null;
			int i = 0;

			while (i < lines.Length)
			{
				Match chunkBeginMatches = beginChunkPatern.Match(lines[i]);
				if (chunkBeginMatches.Success)
				{
					if (currentChunk != null)
						chunks.Add(currentChunk);
					currentChunk = new DiffChunk();
					currentChunk.FirstBegin = Int32.Parse(chunkBeginMatches.Groups["g1"].Value);
					currentChunk.FirstEnd = Int32.Parse(chunkBeginMatches.Groups["g2"].Value);
					currentChunk.SecondBegin = Int32.Parse(chunkBeginMatches.Groups["g3"].Value);
					currentChunk.SecondEnd = Int32.Parse(chunkBeginMatches.Groups["g4"].Value);
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

	public enum DiffActionType
	{
		None,
		Added,
		Updated,
		Deleted
	}

	public class DiffLineData
	{
		string line;
		DiffActionType action;
		int lineNumber;

		public DiffLineData(string line, DiffActionType action)
		{
			this.line = line;
			this.action = action;
		}

		public DiffLineData(string line, DiffActionType action, int lineNumber)
		{
			this.line = line;
			this.action = action;
			this.lineNumber = lineNumber;
		}


		public string FomatedLineNumber
		{
			get
			{
				if (lineNumber < 0)
					return "     ";
				string val = (lineNumber + 1).ToString();
				while (val.Length < 5)
				{
					val = " " + val;
				}
				return val;
			}
		}

		public int LineNumber
		{
			get { return lineNumber; }
			set { lineNumber = value; }
		}

		public string Line
		{
			get { return line; }
			set { line = value; }
		}

		public DiffActionType Action
		{
			get { return action; }
			set { action = value; }
		}
	}

	public class DiffToHTML
	{
		string[] parsedOriginaFile;
		readonly List<DiffLineData> leftPan = new List<DiffLineData>();
		readonly List<DiffLineData> rightPan = new List<DiffLineData>();

		public List<DiffLineData> LeftPan
		{
			get { return leftPan; }
		}

		public List<DiffLineData> RightPan
		{
			get { return rightPan; }
		}

		public DiffToHTML()
		{
		}

		public void Parse(string originalFile, string _unidiffResult)
		{
			originalFile = originalFile.Replace("\r", "");
			parsedOriginaFile = originalFile.Split(new char[] { '\n' });
			List<DiffChunk> chunks = ChunkParser.Parse(_unidiffResult);

			int leftLast = 1;

			foreach (DiffChunk chunk in chunks)
			{
				CopyLines(leftPan, leftLast, chunk.FirstBegin);
				CopyLines(rightPan, leftLast, chunk.FirstBegin);
				leftLast = chunk.FirstEnd + 1;

				//CopyLines(rightPan, rightLast, chunk.SecondBegin);
				//rightLast = chunk.SecondEnd;

				foreach (string line in chunk.lines)
				{
					ProcessDiffLine(line);
				}
			}
			CopyLines(leftPan, leftLast, parsedOriginaFile.Length);
			CopyLines(rightPan, leftLast, parsedOriginaFile.Length);
			PostProcessPans();
		}

		public void Parse2(string baseSources, string diffSource)
		{
			Diff.Item[] diff = Diff.DiffText(baseSources, diffSource, true, true, false);
			baseSources = baseSources.Replace("\r", "");
			diffSource = diffSource.Replace("\r", "");
			string[] aLines = baseSources.Split('\n');
			string[] bLines = diffSource.Split('\n');

			int n = 0;
			int leftLineNumber = 0;
			int rightLineNumber = 0;

			for (int diffItem = 0; diffItem < diff.Length; diffItem++)
			{
				Diff.Item aItem = diff[diffItem];

				// write unchanged lines
				while ((n < aItem.StartB) && (n < bLines.Length))
				{
					leftPan.Add(new DiffLineData(bLines[n], DiffActionType.None, leftLineNumber));
					rightPan.Add(new DiffLineData(bLines[n], DiffActionType.None, rightLineNumber));
					n++;
					leftLineNumber++;
					rightLineNumber++;
				} // while


				for (int m = 0; m < aItem.deletedA; m++)
				{
					leftPan.Add(new DiffLineData(aLines[aItem.StartA + m], DiffActionType.Deleted, leftLineNumber));
					rightPan.Add(new DiffLineData("", DiffActionType.None, -1));
					leftLineNumber++;
				} // for

				// write inserted lines
				while (n < aItem.StartB + aItem.insertedB)
				{
					rightPan.Add(new DiffLineData(bLines[n], DiffActionType.Added, rightLineNumber));
					leftPan.Add(new DiffLineData("", DiffActionType.None, -1));
					n++;
					rightLineNumber++;
				} // while
			} // while

			// write rest of unchanged lines
			while (n < bLines.Length)
			{
				leftPan.Add(new DiffLineData(bLines[n], DiffActionType.None, leftLineNumber));
				rightPan.Add(new DiffLineData(bLines[n], DiffActionType.None, rightLineNumber));
				leftLineNumber++;
				rightLineNumber++;
				n++;
			} // while			

			PostProcessPans();
		}

		void PostProcessPans()
		{
			int i = 0;

			while (i < leftPan.Count)
			{
				int removedLines = CountOfLinese(DiffActionType.Deleted, i);
				if (removedLines > 0)
				{
					int addedLines = CountOfLinese(DiffActionType.Added, i + removedLines);
					int offset = 0;
					int count = removedLines;
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
						MakeLinesUpdated(i + offset, count * 2);
						i += count;
					}
				}
				else
					i++;
			}
		}

		private void MakeLinesUpdated(int position, int count)
		{
			int leftPos = position;
			int rightPos = position;

			for (int i = 0; i < count; i++)
			{
				if (leftPan[leftPos].Action == DiffActionType.Deleted)
				{
					leftPan[leftPos].Action = DiffActionType.Updated;
					rightPan.RemoveAt(rightPos);
					leftPos++;
					continue;
				}

				if (rightPan[rightPos].Action == DiffActionType.Added)
				{
					rightPan[rightPos].Action = DiffActionType.Updated;
					leftPan.RemoveAt(leftPos);
					rightPos++;
					continue;
				}
			}
		}

		private int CountOfLinese(DiffActionType action, int i)
		{
			int count = 0;
			while (i < leftPan.Count)
			{
				if (leftPan[i].Action == action || rightPan[i].Action == action)
					count++;
				else
					break;
				i++;
			}
			return count;
		}

		private void ProcessDiffLine(string line)
		{
			if (line.Length == 0)
				line = " ";

			string resultString = line.Substring(1);
			char controlSymbol = line[0];

			if (controlSymbol == ' ')
			{
				leftPan.Add(new DiffLineData(resultString, DiffActionType.None));
				rightPan.Add(new DiffLineData(resultString, DiffActionType.None));
			}
			if (controlSymbol == '-')
			{
				leftPan.Add(new DiffLineData(resultString, DiffActionType.Deleted));
				rightPan.Add(new DiffLineData("", DiffActionType.None));
			}
			if (controlSymbol == '+')
			{
				leftPan.Add(new DiffLineData("", DiffActionType.None));
				rightPan.Add(new DiffLineData(resultString, DiffActionType.Added));
			}
		}

		void CopyLines(List<DiffLineData> pan, int firstLine, int lastLine)
		{
			for (int i = firstLine - 1; i < lastLine - 1; i++)
			{
				pan.Add(new DiffLineData(parsedOriginaFile[i], DiffActionType.None));
			}
		}
	}
}
