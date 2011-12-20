// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Subversion.SerializationPatches.Xml
{
	internal class NewRevisionRangeDetectedLocalMessagePatch : IPatch
	{
		public bool NeedToApply(string text)
		{
			return text.Contains(">Tp.Subversion.Workflow.NewRevisionRangeDetectedLocalMessage<") &&
			       text.Contains(">Tp.Subversion<") && text.Contains(">Tp.Subversion.VersionControlSystem.RevisionRange<") &&
			       text.Contains(">Tp.Subversion.Subversion.SvnRevisionId<");
		}

		public string Apply(string text)
		{
			return text.Replace(">Tp.Subversion.Workflow.NewRevisionRangeDetectedLocalMessage<",
			                    ">Tp.SourceControl.Workflow.Workflow.NewRevisionRangeDetectedLocalMessage<")
				.Replace(">Tp.Subversion<", ">Tp.SourceControl<")
				.Replace(">System.Int64<", ">System.String<")
				.Replace(">Tp.Subversion.VersionControlSystem.RevisionRange<",
				         ">Tp.SourceControl.VersionControlSystem.RevisionRange<")
				.Replace(">Tp.Subversion.Subversion.SvnRevisionId<", ">Tp.SourceControl.VersionControlSystem.RevisionId<");
		}
	}
}