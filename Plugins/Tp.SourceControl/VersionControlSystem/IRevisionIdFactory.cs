//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.SourceControl.VersionControlSystem
{
	public interface IRevisionIdComparer
	{
		ICompareRevisionSecondArg Is(RevisionId firstArg);
		ICompareRevisionSecondArg Does(RevisionId firstArg);
		RevisionId FindMinFromRevision(RevisionRange[] revisionRanges);
		RevisionId FindMaxToRevision(RevisionRange[] revisionRanges);
		RevisionId ConvertToRevisionId(string startRevision);
	}

	public interface ICompareRevisionSecondArg
	{
		bool Before(RevisionRange revisionRange);

		bool Behind(RevisionRange revisionRange);
		bool Belong(RevisionRange revisionRange);
		bool GreaterThan(RevisionId revisionId);
		bool LessThan(RevisionId revisionId);
	}
}