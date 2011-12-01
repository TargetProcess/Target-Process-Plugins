// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tp.Integration.Messages.EntityLifecycle;

// ReSharper disable CheckNamespace
namespace Tp.Integration.Testing.Common
// ReSharper restore CheckNamespace
{
	public static class MessageQueueExtensions
	{
		public static IEnumerable<TDto> GetCreatedDtos<TDto>(this MessageQueue messageQueue)
		{
			return messageQueue.GetMessages<CreateCommand>()
				.Select(x => x.Dto)
				.OfType<TDto>();
		}
	}
}