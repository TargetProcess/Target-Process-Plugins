// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.SourceControl.Comments.Actions;
using Tp.SourceControl.Messages;

namespace Tp.SourceControl
{
	public interface IActionVisitor
	{
		void Accept(PostTimeAction action);
		void Accept(AssignRevisionToEntityAction action);
		void Accept(PostCommentAction action);
		void Accept(ChangeStatusAction action);
	}
}