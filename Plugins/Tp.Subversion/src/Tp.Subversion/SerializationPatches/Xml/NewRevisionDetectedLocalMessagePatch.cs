// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Subversion.SerializationPatches.Xml
{
    public class NewRevisionDetectedLocalMessagePatch : IPatch
    {
        public bool NeedToApply(string text)
        {
            return text.Contains(">Tp.Subversion.Messages.NewRevisionDetectedLocalMessage<") &&
                text.Contains(">Tp.Subversion<") && text.Contains(">Tp.Subversion.VersionControlSystem.RevisionInfo<") &&
                text.Contains(">Tp.Subversion.Subversion.SvnRevisionId<") &&
                text.Contains(">Tp.Subversion.VersionControlSystem.RevisionEntryInfo<");
        }

        public string Apply(string text)
        {
            return text.Replace(">Tp.Subversion.Messages.NewRevisionDetectedLocalMessage<",
                    ">Tp.SourceControl.Messages.NewRevisionDetectedLocalMessage<")
                .Replace(">Tp.Subversion<", ">Tp.SourceControl<")
                .Replace(">System.Int64<", ">System.String<")
                .Replace(">Tp.Subversion.VersionControlSystem.RevisionInfo<", ">Tp.SourceControl.VersionControlSystem.RevisionInfo<")
                .Replace(">Tp.Subversion.Subversion.SvnRevisionId<", ">Tp.SourceControl.VersionControlSystem.RevisionId<")
                .Replace(">Tp.Subversion.VersionControlSystem.RevisionEntryInfo<",
                    ">Tp.SourceControl.VersionControlSystem.RevisionEntryInfo<")
                .Replace(">Tp.Subversion.VersionControlSystem.RevisionEntryInfo[]<",
                    ">Tp.SourceControl.VersionControlSystem.RevisionEntryInfo[]<");
        }
    }
}
