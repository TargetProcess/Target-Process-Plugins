// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Text;
using System;

namespace Tp.Mercurial.VersionControlSystem
{
	public class InvalidRevisionException : Exception
	{
        private static readonly string Msg = string.Format(
            "should be between {0} and {1}", 
            MercurialRevisionId.UtcTimeMin.ToShortDateString(),
            MercurialRevisionId.UtcTimeMax.ToShortDateString());

		public override string Message
		{
			get { return Msg; }
		}
	}
}