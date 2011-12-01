// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Integration.Plugin.Common.Domain
{
	/// <summary>
	/// Manages access to current profile object and storage for current profile.
	/// </summary>
	public interface IProfile : IProfileReadonly
	{
		new object Settings { get; set; }
		IActivityLog Log { get; }

		void Save();
		void MarkAsInitialized();
		void MarkAsNotInitialized();
	}
}