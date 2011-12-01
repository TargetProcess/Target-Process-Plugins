// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

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