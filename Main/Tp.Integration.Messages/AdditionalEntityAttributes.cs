// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Common
{
	public partial class BuildDTO
	{
		[ForeignKey]
		public int? ProjectID { get; set; }

		[RelationName]
		public string ProjectName { get; set; }
	}

	public partial class BugDTO
	{
		/// <summary>
		/// Comment on changing state
		/// </summary>
		public string CommentOnChangingState { get; set; }
	}

	public partial class AssignableDTO
	{
		/// <summary>
		/// Comment on changing state
		/// </summary>
		public string CommentOnChangingState { get; set; }
	}

	public partial class TestPlanRunDTO
	{
		/// <summary>
		/// Comment on changing state
		/// </summary>
		public string CommentOnChangingState { get; set; }
	}

	public partial class RequestDTO
	{
		/// <summary>
		/// Comment on changing state
		/// </summary>
		public string CommentOnChangingState { get; set; }
	}

	public partial class UserStoryDTO
	{
		/// <summary>
		/// Comment on changing state
		/// </summary>
		public string CommentOnChangingState { get; set; }
	}

	public partial class TaskDTO
	{
		/// <summary>
		/// Comment on changing state
		/// </summary>
		public string CommentOnChangingState { get; set; }
	}

	public partial class AttachmentDTO
	{
		/// <summary>
		/// Size of file
		/// </summary>
		public long? FileSize { get; set; }
	}
}