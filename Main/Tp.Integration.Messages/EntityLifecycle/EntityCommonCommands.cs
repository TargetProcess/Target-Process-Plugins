// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
	[Serializable]
	public class CreateCommand : ITargetProcessCommand
	{
		public DataTransferObject Dto { get; set; }
	}

	[Serializable]
	public class UpdateCommand : ITargetProcessCommand
	{
		public UpdateCommand()
		{
			ChangedFields = new string[] {};
		}

		public DataTransferObject Dto { get; set; }
		public string[] ChangedFields { get; set; }
	}

	[Serializable]
	public class DeleteCommand : ITargetProcessCommand
	{
		public int Id { get; set; }
		public DtoType DtoType { get; set; }
	}
}