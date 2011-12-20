// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Subversion.SerializationPatches.Xml
{
	public class SvnActionPatch : IPatch
	{
		public bool NeedToApply(string text)
		{
			return text.Contains(">Tp.Subversion.Messages.AssignRevisionToEntityAction<") && text.Contains(">Tp.Subversion<");
		}

		public string Apply(string text)
		{
			return text.Replace(">Tp.Subversion<", ">Tp.SourceControl<")
				.Replace(">Tp.Subversion.Messages.AssignRevisionToEntityAction<", ">Tp.SourceControl.Messages.AssignRevisionToEntityAction<")
				.Replace(">Tp.Subversion.Comments.Actions.PostTimeAction<", ">Tp.SourceControl.Comments.Actions.PostTimeAction<")
				.Replace(">Tp.Subversion.Comments.Actions.ChangeStatusAction<", ">Tp.SourceControl.Comments.Actions.ChangeStatusAction<")
				.Replace(">Tp.Subversion.Comments.Actions.PostCommentAction<", ">Tp.SourceControl.Comments.Actions.PostCommentAction<")
				.Replace("Tp.Subversion.Comments.IAction, Tp.Subversion", "Tp.SourceControl.Comments.IAction, Tp.SourceControl");
		}
	}
}