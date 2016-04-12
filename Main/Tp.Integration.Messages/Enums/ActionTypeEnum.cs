namespace Tp.Integration.Common
{
	/// <summary>
	/// Describes the action types 
	/// </summary>
	public enum ActionTypeEnum
	{
		/// <summary>
		/// Nothing changes
		/// </summary>
		None = -1,

		/// <summary>
		/// The entity is added
		/// </summary>
		Add = 0,

		/// <summary>
		/// The entity is updated
		/// </summary>
		Update = 1,

		/// <summary>
		/// The entity is deleted
		/// </summary>
		Delete = 2,

		/// <summary>
		/// The state of the entity is changed
		/// </summary>
		ChangeState = 3,

		/// <summary>
		/// The assignments are changed
		/// </summary>
		Assign = 4,

		/// <summary>
		/// The comment is added
		/// </summary>
		AddComment = 5
	}
}
