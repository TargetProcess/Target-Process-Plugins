//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus.Saga;

namespace Tp.SourceControl
{
	public class NewProfileInitializationSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public int AllUsersCount { get; set; }

		public int UsersRetrievedCount { get; set; }
	}
}