// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Plugin.Common
{
	/// <summary>
	/// Represents a bus for sending commands to TargetProcess.
	/// </summary>
	public interface ICommandBus
	{
		/// <summary>
		/// Sends create entity commands to TargetProcess
		/// </summary>
		/// <param name="createCommands">Create entity commands</param>
		/// <returns>Objects of this interface are returned from calling IBus.Send. The interface allows the caller to register for a callback when a response is received to their original call to IBus.Send.</returns>
		ICallback Send(params ICreateEntityCommand<DataTransferObject>[] createCommands);

		/// <summary>
		/// Sends update entity commands to TargetProcess
		/// </summary>
		/// <param name="updateCommands">Update entity commands</param>
		/// <returns>Objects of this interface are returned from calling IBus.Send. The interface allows the caller to register for a callback when a response is received to their original call to IBus.Send.</returns>
		ICallback Send(params IUpdateEntityCommand<DataTransferObject>[] updateCommands);

		/// <summary>
		/// Sends delete entity commands to TargetProcess
		/// </summary>
		/// <param name="deleteCommands">Delete entity commands</param>
		/// <returns>Objects of this interface are returned from calling IBus.Send. The interface allows the caller to register for a callback when a response is received to their original call to IBus.Send.</returns>
		ICallback Send(params IDeleteEntityCommand[] deleteCommands);
	}
}