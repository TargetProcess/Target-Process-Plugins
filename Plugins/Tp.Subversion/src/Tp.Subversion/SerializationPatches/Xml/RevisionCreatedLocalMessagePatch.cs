// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Subversion.SerializationPatches.Xml
{
    public class RevisionCreatedLocalMessagePatch : IPatch
    {
        public bool NeedToApply(string text)
        {
            return text.Contains(">Tp.Subversion.Messages.RevisionCreatedLocalMessage<") && text.Contains(">Tp.Subversion<");
        }

        public string Apply(string text)
        {
            return text.Replace(">Tp.Subversion.Messages.RevisionCreatedLocalMessage<",
                    ">Tp.SourceControl.Messages.RevisionCreatedLocalMessage<")
                .Replace(">Tp.Subversion<", ">Tp.SourceControl<");
        }
    }
}
