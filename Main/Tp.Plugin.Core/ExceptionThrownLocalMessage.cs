// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.TargetProcessLifecycle;

namespace Tp.Plugin.Core
{
	[Serializable]
	public class ExceptionThrownLocalMessage : TargetProcessExceptionThrownMessage, IPluginLocalMessage
	{
		public static ExceptionThrownLocalMessage Create(TargetProcessExceptionThrownMessage message, Guid sagaId)
		{
			return new ExceptionThrownLocalMessage {ExceptionString = message.ExceptionString, SagaId = sagaId};
		}
	}
}