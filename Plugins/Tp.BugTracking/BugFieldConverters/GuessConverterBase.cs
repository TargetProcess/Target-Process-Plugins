// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.BugTracking.BugFieldConverters
{
	public abstract class GuessConverterBase<T, TDto> : IBugConverter<T> where TDto : DataTransferObject
	{
		private readonly IStorageRepository _storageRepository;
		protected readonly IActivityLogger _logger;

		protected GuessConverterBase(IStorageRepository storageRepository, IActivityLogger logger)
		{
			_storageRepository = storageRepository;
			_logger = logger;
		}

		protected IStorageRepository StorageRepository
		{
			get { return _storageRepository; }
		}

		public void Apply(T thirdPartyBug, ConvertedBug convertedBug)
		{
			var value = GetThirdPartyValue(thirdPartyBug);

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
				_logger.ErrorFormat("{0} mapping failed. {1}; Value: {2}", BugFieldName, thirdPartyBug.ToString(), value);
			}
		}

		protected abstract TDto GetFromStorage(string value);

		protected abstract void SetValue(ConvertedBug convertedBug, int id);

		protected abstract string GetThirdPartyValue(T thirdPartyBug);

		protected abstract MappingContainer Map { get; }

		protected abstract BugField BugField { get; }

		protected abstract string BugFieldName { get; }

		protected IStorage<TDto> GetDtoStorage()
		{
			return _storageRepository.Get<TDto>();
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