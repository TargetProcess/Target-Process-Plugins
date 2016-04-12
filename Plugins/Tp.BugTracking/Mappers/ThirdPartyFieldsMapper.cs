// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.BugTracking.Commands.Dtos;
using Tp.BugTracking.Settings;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.BugTracking.Mappers
{
	public class ThirdPartyFieldsMapper : IThirdPartyFieldsMapper
	{
		private readonly IStorageRepository _storageRepository;

		public ThirdPartyFieldsMapper(IStorageRepository storageRepository)
		{
			_storageRepository = storageRepository;
		}

		public Mappings Map(MappingSource source)
		{
			return new Mappings
			       	{
			       		States = GetMappingFor(source.States, GetMappedStates()),
			       		Severities = GetMappingFor(source.Severities, GetMappedSeverities()),
			       		Priorities = GetMappingFor(source.Priorities, GetMappedPriorities())
			       	};
		}

		private static MappingContainer GetMappingFor(MappingSourceEntry source, IEnumerable<MappingElement> mapped)
		{
			var elements = new List<MappingElement>();
			IEnumerable<string> thirdPartyItems = source.ThirdPartyItems;

			var existingMappingElements = mapped
				.FilterOutObsoleteItemsBy(thirdPartyItems)
				.FilterOutEmptyItems();

			elements.AddRange(existingMappingElements);

			var newMappingElements = thirdPartyItems
				.FilterOutDuplicateThirdPartyItemsBy(elements)
				.ToMappingElements(source);
			elements.AddRange(newMappingElements);

			var container = new MappingContainer();
			container.AddRange(elements.OrderBy(r => r.Key).ToList());

			return container;
		}

		private IEnumerable<MappingElement> GetMappedStates()
		{
			return GetMappedEntities(x => x.StatesMapping);
		}

		private IEnumerable<MappingElement> GetMappedSeverities()
		{
			return GetMappedEntities(x => x.SeveritiesMapping);
		}

		private IEnumerable<MappingElement> GetMappedPriorities()
		{
			return GetMappedEntities(x => x.PrioritiesMapping);
		}

		private IEnumerable<MappingElement> GetMappedEntities(Func<IBugTrackingMappingSource, IEnumerable<MappingElement>> selector)
		{
			return _storageRepository.IsNull
					? Enumerable.Empty<MappingElement>()
					: selector(_storageRepository.GetProfile<IBugTrackingMappingSource>());
		} 
	}
}