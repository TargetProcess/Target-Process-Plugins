using System;
using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Messages.Commands
{
	/// <summary>
	/// Represents a plugin command response
	/// </summary>
	[Serializable]
	public class PluginCommandResponseMessage : ITargetProcessMessage
	{
		/// <summary>
		/// Response data that will be passed back to UI
		/// </summary>
		public string ResponseData { get; set; }

		/// <summary>
		/// Success if command was executed successfully. Fail if something was wrong.
		/// </summary>
		public PluginCommandStatus PluginCommandStatus { get; set; }

		public override string ToString()
		{
			return "Response status {0}: {1}".Fmt(PluginCommandStatus, ResponseData);
		}
	}

	public enum PluginCommandStatus
	{
		/// <summary>
		/// Succeed
		/// </summary>
		Succeed,

		/// <summary>
		/// Some errors/exceptions occured
		/// </summary>
		Error,

		/// <summary>
		/// Failed, but without errors/exceptions
		/// </summary>
		Fail
	}
}
