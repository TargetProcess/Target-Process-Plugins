// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Messages.EntityLifecycle
{
	/// <summary>
	/// Marker interface to indicate that a class represents a message that can be involved into some saga(long-running process).
	/// </summary>
	public interface ISagaMessage : ITargetProcessMessage
	{
		Guid SagaId { get; set; }
	}
}