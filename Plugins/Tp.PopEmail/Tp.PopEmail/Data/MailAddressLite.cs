// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.PopEmailIntegration.Data
{
	[Serializable]
	public class MailAddressLite
	{
		public string Address { get; set; }
		public string DisplayName { get; set; }
	}
}
