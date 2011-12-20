// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.Synchronizer
{
	public abstract class EntityChangedHandler<TDto> where TDto : DataTransferObject, new()
	{
		private readonly IStorageRepository _storage;

		protected EntityChangedHandler(IStorageRepository storage)
		{
			_storage = storage;
		}

		protected virtual bool AreEqual(TDto dtoInStage, TDto dtoOfTp)
		{
			return dtoInStage.ID == dtoOfTp.ID;
		}

		protected IStorage<TDto> EntitiesStorage
		{
			get { return _storage.Get<TDto>(); }
		}

		protected IStorageRepository Storage
		{
			get { return _storage; }
		}

		protected void Create(TDto entity)
		{
			if (!NeedToProcess(entity)) return;

			EntitiesStorage.Add(entity);
		}

		protected void Update(TDto entity)
		{
			if (!NeedToProcess(entity)) return;

			Delete(entity);
			EntitiesStorage.Add(entity);
		}

		protected void Delete(TDto entity)
		{
			if (!NeedToProcess(entity)) return;

			EntitiesStorage.Remove(x => AreEqual(x, entity));
		}

		protected virtual bool NeedToProcess(TDto dto)
		{
			return true;
		}

		protected BugzillaProfile Profile
		{
			get { return _storage.GetProfile<BugzillaProfile>(); }
		}
	}
}