namespace Tp.Integration.Common
{
	/// <summary>
	/// Describes modification types for Audit History
	/// </summary>
	public enum ModificationTypeEnum
	{
		/// <summary>
		/// Undefined
		/// </summary>
		None,

		/// <summary>
		/// The entity is created
		/// </summary>
		Created,

		/// <summary>
		/// The entity is updated
		/// </summary>
		Updated,

		/// <summary>
		/// The entity is deleted
		/// </summary>
		Deleted,

		/// <summary>
		/// The project member is added
		/// </summary>
		ProjectMemberAdded,

		/// <summary>
		/// The project member is deleted
		/// </summary>
		ProjectMemberDeleted,

		/// <summary>
		/// The attachment is added
		/// </summary>
		AttachmentAdded,

		/// <summary>
		/// The attachment is deleted
		/// </summary>
		AttachmentDeleted,

		/// <summary>
		/// The comment is added
		/// </summary>
		CommentAdded,

		/// <summary>
		/// The comment is deleted
		/// </summary>
		CommentDeleted,

		/// <summary>
		/// The state is changed
		/// </summary>
		StateChanged,

		/// <summary>
		/// The assignment is added
		/// </summary>
		AssignmentAdded,

		/// <summary>
		/// The assignment is deleted
		/// </summary>
		AssignmentDeleted
	}
}
