// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Bugzilla.BugFieldConverters
{
	public class CommentConverter : IBugConverter
	{
		public const string StateIsChangedComment = "State is changed by Bugzilla plugin";

		public void Apply(BugzillaBug bugzillaBug, ConvertedBug convertedBug)
		{
			
			convertedBug.BugDto.CommentOnChangingState = StateIsChangedComment;
			
		}
	}
}