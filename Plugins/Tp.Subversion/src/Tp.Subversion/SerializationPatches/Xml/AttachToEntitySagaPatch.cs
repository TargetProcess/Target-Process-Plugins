// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Subversion.SerializationPatches.Xml
{
    public class AttachToEntitySagaPatch : IPatch
    {
        public bool NeedToApply(string text)
        {
            return text.Contains("Tp.Subversion.Workflow.AttachToEntitySagaData, Tp.Subversion") &&
                text.Contains("AttachToEntitySagaData:#Tp.Subversion.Workflow");
        }

        public string Apply(string text)
        {
            return text.Replace("Tp.Subversion.Workflow.AttachToEntitySagaData, Tp.Subversion",
                    "Tp.SourceControl.Workflow.Workflow.AttachToEntitySagaData, Tp.SourceControl")
                .Replace("AttachToEntitySagaData:#Tp.Subversion.Workflow", "AttachToEntitySagaData:#Tp.SourceControl.Workflow.Workflow");
        }
    }
}
