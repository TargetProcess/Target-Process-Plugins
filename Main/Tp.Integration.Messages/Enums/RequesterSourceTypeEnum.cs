namespace Tp.Integration.Common
{
	/// <summary>
	/// Describes the way the requester was added
	/// </summary>
	public enum RequesterSourceTypeEnum
	{
		/// <summary>
		/// Undefined
		/// </summary>
		None = 0,

		/// <summary>
		/// By mail
		/// </summary>
		Mail = 1,

		/// <summary>
		/// Somehow externally
		/// </summary>
		External = 2,

		/// <summary>
		/// Right from the TargetProcess system
		/// </summary>
		Internal = 3,
	}
}
