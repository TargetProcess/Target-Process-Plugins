// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Subversion.SerializationPatches.Xml
{
    public class CreateRevisionSagaPatch : IPatch
    {
        public bool NeedToApply(string text)
        {
            return text.Contains(">Tp.Subversion.Workflow.CreateRevisionSagaData, Tp.Subversion<") &&
                text.Contains("CreateRevisionSagaData:#Tp.Subversion.Workflow");
        }

        public string Apply(string text)
        {
            return text.Replace(">Tp.Subversion.Workflow.CreateRevisionSagaData, Tp.Subversion<",
                    ">Tp.SourceControl.Workflow.Workflow.CreateRevisionSagaData, Tp.SourceControl<")
                .Replace("CreateRevisionSagaData:#Tp.Subversion.Workflow", "CreateRevisionSagaData:#Tp.SourceControl.Workflow.Workflow");
        }
    }
}
