// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.BugFieldConverters
{
	public abstract class GuessConverter<TEntity> : IBugConverter where TEntity : DataTransferObject
	{
		private readonly IStorageRepository _storageRepository;

		protected GuessConverter(IStorageRepository storageRepository)
		{
			_storageRepository = storageRepository;
		}

		public void Apply(BugzillaBug bugzillaBug, ConvertedBug convertedBug)
		{
			var value = GetBugzillaValue(bugzillaBug);

			if (string.IsNullOrEmpty(value))
				return;

			if (Map != null && Map[value] != null)
			{
				SetFieldFromMapping(value, convertedBug);
			}
			else
			{
				SetFieldFromStorage(value, convertedBug);
			}
		}

		protected abstract TEntity GetFromStorage(string value);

		protected abstract void SetValue(ConvertedBug convertedBug, int id);

		protected abstract string GetBugzillaValue(BugzillaBug bugzillaBug);

		protected abstract MappingContainer Map { get; }

		protected abstract BugField BugField { get; }

		protected BugzillaProfile Profile
		{
			get { return _storageRepository.GetProfile<BugzillaProfile>(); }
		}

		protected IStorage<TEntity> GetStorage()
		{
			return _storageRepository.Get<TEntity>();
		}

		protected void SetFieldFromMapping(string value, ConvertedBug convertedBug)
		{
			var mappedValue = Map[value];
			if (mappedValue.Id != 0)
			{
				SetValue(convertedBug, mappedValue.Id);
				convertedBug.ChangedFields.Add(BugField);
			}
		}

		protected void SetFieldFromStorage(string value, ConvertedBug convertedBug)
		{
			var state = GetFromStorage(value);

			if (state != null)
			{
				SetValue(convertedBug, state.ID.GetValueOrDefault());
				convertedBug.ChangedFields.Add(BugField);
			}
		}
	}
}
