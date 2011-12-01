// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Messages.Commands
{
	public class ChangeEntityStateCommand : ITargetProcessCommand
	{
		public int? EntityId { get; set; }
		public string State { get; set; }
		public int UserID { get; set; }
		public string Comment { get; set; }
		public string DefaultComment { get; set; }
	}
}