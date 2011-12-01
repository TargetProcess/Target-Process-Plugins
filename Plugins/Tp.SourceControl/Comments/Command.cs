// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;

namespace Tp.SourceControl.Comments
{
	public class Command
	{
		private IList<string> _action = new List<string>();
		private IList<int> _entityID = new List<int>();
		private IList<TimeCommand> _times = new List<TimeCommand>();
		private IList<string> _comments = new List<string>();

		public IList<TimeCommand> Times
		{
			get { return _times; }
			set { _times = value; }
		}

		public IList<string> CommandActions
		{
			get { return _action; }
			set { _action = value; }
		}

		public IList<int> EntityIDs
		{
			get { return _entityID; }
			set { _entityID = value; }
		}

		public IList<string> Comments
		{
			get { return _comments; }
			set { _comments = value; }
		}
	}
}