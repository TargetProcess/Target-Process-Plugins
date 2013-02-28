// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Tfs.VersionControlSystem
{
	public class InvalidRevisionException : Exception
	{
		private const string Msg = "Specify a start revision number in the range of 1 - 2147483647.";

		public override string Message
		{
			get { return Msg; }
		}
	}
}