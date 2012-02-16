// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Domain
{
	/// <summary>
	/// Provides access to all profiles.
	/// Injected into StructureMap container.
	/// </summary>
	public interface IProfileCollection : IEnumerable<IProfile>
	{
		/// <summary>
		/// Retrieves profile by name.
		/// </summary>
		/// <param name="profileName">Profile name</param>
		/// <returns>Profile</returns>
		IProfile this[ProfileName profileName] { get; }

		/// <summary>
		/// Removes profile.
		/// </summary>
		/// <param name="profile">Profile to remove.</param>
		void Remove(IProfile profile);

		/// <summary>
		/// Adds profile.
		/// </summary>
		/// <param name="profileCreationArgs">Args to create profile with.</param>
		/// <returns>Created profile.</returns>
		IProfile Add(ProfileCreationArgs profileCreationArgs);
	}
}