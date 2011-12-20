// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Subversion.SerializationPatches.Xml
{
	public class RevisionRangeInStoragePatch : IPatch
	{
		public string Apply(string text)
		{
			return text.Replace(">Tp.Subversion.VersionControlSystem.RevisionRange, Tp.Subversion<", ">Tp.SourceControl.VersionControlSystem.RevisionRange, Tp.SourceControl<")
				   .Replace("RevisionRange:#Tp.Subversion.VersionControlSystem", "RevisionRange:#Tp.SourceControl.VersionControlSystem")
				   .Replace(@"""__type"":""SvnRevisionId:#Tp.Subversion.Subversion"",", "")
				   .Replace(@"""_value""", @"""Value""");
		}

		public bool NeedToApply(string text)
		{
			return text.Contains(">Tp.Subversion.VersionControlSystem.RevisionRange, Tp.Subversion<") &&
			       text.Contains("RevisionRange:#Tp.Subversion.VersionControlSystem") &&
			       text.Contains("SvnRevisionId:#Tp.Subversion.Subversion") &&
				   text.Contains(@"""_value""");
		}
	}
}