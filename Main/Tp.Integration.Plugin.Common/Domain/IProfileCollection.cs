// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
	public interface IProfileCollection : IEnumerable<IProfile>
	{
		IProfile this[ProfileName profileName] { get; }
		void Remove(IProfile profile);
		IProfile Add(ProfileCreationArgs profileCreationArgs);
	}
}