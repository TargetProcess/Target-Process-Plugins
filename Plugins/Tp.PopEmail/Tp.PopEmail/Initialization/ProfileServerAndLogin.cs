// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.PopEmailIntegration.Initialization
{
	[Serializable]
	public class ProfileServerAndLogin
	{
		public string MailServer { get; set; }
		public string Login { get; set; }
	}
}