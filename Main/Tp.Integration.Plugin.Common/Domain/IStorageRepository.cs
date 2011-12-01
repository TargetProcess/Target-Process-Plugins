// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Core;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.Domain
{
	/// <summary>
	/// Provides access to storage of objects by type. Storage is profile specific.
	/// </summary>
	public interface IStorageRepository : INullable
	{
		/// <summary>
		/// Gets storage for objects by type.
		/// </summary>
		/// <typeparam name="T">The type of object which storage is returned.</typeparam>
		/// <returns>The storage for provided type</returns>
		IStorage<T> Get<T>();

		/// <summary>
		/// Gets named storage for objects by type
		/// </summary>
		/// <typeparam name="T">The type of object which storage is returned.</typeparam>
		/// <param name="storageNames"></param>
		/// <returns>The storage for provided type</returns>
		/// <returns></returns>
		IStorage<T> Get<T>(params StorageName[] storageNames);

		/// <summary>
		/// Gets current profile object by type.
		/// </summary>
		/// <typeparam name="T">The plugin profile type.</typeparam>
		/// <returns>Current Profile object.</returns>
		T GetProfile<T>();
	}
}