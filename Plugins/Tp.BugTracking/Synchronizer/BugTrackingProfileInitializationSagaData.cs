// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus.Saga;

namespace Tp.BugTracking.Synchronizer
{
	public class BugTrackingProfileInitializationSagaData : ISagaEntity
	{
		public Guid Id { get; set; }
		public string Originator { get; set; }
		public string OriginalMessageId { get; set; }

		public int EntityStatesRetrievedCount { get; set; }

		public int AllEntityStatesCount { get; set; }

		public int AllSeveritiesCount { get; set; }

		public int SeveritiesRetrievedCount { get; set; }

		public int AllPrioritiesCount { get; set; }

		public int PrioritiesRetrievedCount { get; set; }

		public int AllProjectsCount { get; set; }

		public int ProjectsRetrievedCount { get; set; }

		public int AllUsersCount { get; set; }

		public int UsersRetrievedCount { get; set; }

		public int AllRolesCount { get; set; }

		public int RolesRetrievedCount { get; set; }
	}
}