namespace Tp.Integration.Common
{
	/// <summary>
	/// Describes role extension for rules. Only for system usage
	/// </summary>
	public enum RoleExtensionEnum
	{
		/// <summary>
		/// Undefined
		/// </summary>
		None = -1,

		/// <summary>
		/// Owner
		/// </summary>
		Owner = 1,

		/// <summary>
		/// Project Member
		/// </summary>
		ProjectMember = 2,

		/// <summary>
		/// Assigned User
		/// </summary>
		AssignedUser = 3
	}
}
