// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.FileStorage;

namespace Tp.Integration.Plugin.Common.Domain
{
	/// <summary>
	/// Manages access to current profile object and storage for current profile.
	/// In case there is no profile in <see cref="PluginContext"/>, SafeNull object will be returned.
	/// Injected into StructureMap container.
	/// </summary>
	public interface IProfile : IProfileReadonly
	{
		/// <summary>
		/// Settings of current profile.
		/// </summary>
		new object Settings { get; set; }

		/// <summary>
		/// Log of current profile.
		/// </summary>
		IActivityLog Log { get; }

		/// <summary>
		/// Profile-specific file storage.
		/// </summary>
		IProfileFileStorage FileStorage { get; }

		/// <summary>
		/// Saves changes made to current profile. You can change Settings and Initialized values.
		/// </summary>
		void Save();

		/// <summary>
		/// Marks current profile as initialized.
		/// </summary>
		void MarkAsInitialized();

		/// <summary>
		/// Marks current profile as not initialized. Profile will not receive notifications from TargetProcess in this state. 
		/// Incoming messages will be stored in database. When profile will be marked as initialized, those stored messages will be immediately processed.
		/// </summary>
		void MarkAsNotInitialized();
	}
}