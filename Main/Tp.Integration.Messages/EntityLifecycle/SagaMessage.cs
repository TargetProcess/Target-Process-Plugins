// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Messages.EntityLifecycle
{
	/// <summary>
	/// Base class for message that can be involved in saga.
	/// </summary>
	[Serializable]
	public class SagaMessage
	{
		public SagaMessage()
		{
			SagaId = Guid.Empty;
		}

		public Guid SagaId { get; set; }
	}
}