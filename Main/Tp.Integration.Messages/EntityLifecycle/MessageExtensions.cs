// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;

namespace Tp.Integration.Messages.EntityLifecycle
{
	public static class MessageExtensions
	{
		public static bool IsMessageOfCrudType(this IMessage message)
		{
			return GetBaseTypes(message.GetType())
				.Where(x => x.IsGenericType)
				.Select(x => x.GetGenericTypeDefinition())
				.Any(x => x == typeof (EntityCreatedMessage<>) || x == typeof (EntityUpdatedMessage<,>) || x == typeof (EntityDeletedMessage<>));
		}

		private static IEnumerable<Type> GetBaseTypes(Type t)
		{
			var parent = t;
			while (parent != null)
			{
				yield return parent;
				parent = parent.BaseType;
			}
		}
	}
}
