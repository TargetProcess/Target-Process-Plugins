// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.Bugzilla.BugFieldConverters
{
	public abstract class GuessConverter<TEntity> : IBugConverter where TEntity : DataTransferObject
	{
		private readonly IStorageRepository _storageRepository;
		protected readonly IActivityLogger _logger;

		protected GuessConverter(IStorageRepository storageRepository, IActivityLogger logger)
		{
			_storageRepository = storageRepository;
			_logger = logger;
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

			if (!convertedBug.ChangedFields.Contains(BugField))
			{
				_logger.ErrorFormat("{0} mapping failed. {1}; Value: {2}", BugFieldName, bugzillaBug.ToString(), value);
			}
		}

		protected abstract TEntity GetFromStorage(string value);

		protected abstract void SetValue(ConvertedBug convertedBug, int id);

		protected abstract string GetBugzillaValue(BugzillaBug bugzillaBug);

		protected abstract MappingContainer Map { get; }

		protected abstract BugField BugField { get; }

		protected abstract string BugFieldName { get; }

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
				_logger.InfoFormat("{0} mapped. Bug: {1}; Value: {2}", BugFieldName, convertedBug.BugDto.Name, value);
			}
		}

		protected void SetFieldFromStorage(string value, ConvertedBug convertedBug)
		{
			var state = GetFromStorage(value);

			if (state != null)
			{
				SetValue(convertedBug, state.ID.GetValueOrDefault());
				convertedBug.ChangedFields.Add(BugField);
				_logger.InfoFormat("{0} guessed. Bug: {1}; Value: {2}", BugFieldName, convertedBug.BugDto.Name, value);
			}
		}
	}
}