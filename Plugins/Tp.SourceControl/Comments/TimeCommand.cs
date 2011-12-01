// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.SourceControl.Comments
{
	public class TimeCommand
	{
		public decimal TimeSpent { get; set; }
		public decimal TimeLeft { get; set; }

		public TimeCommand(decimal timeSpent, decimal timeLeft)
		{
			TimeSpent = timeSpent;
			TimeLeft = timeLeft;
		}
	}
}