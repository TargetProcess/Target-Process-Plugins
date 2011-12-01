using System.Collections.Generic;
using System.Linq;
using Tp.Bugzilla.CustomCommand.Dtos;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.Mappers
{
	public class BugzillaFieldsMapper : IBugzillaFieldsMapper
	{
		private readonly IStorageRepository _storageRepository;

		public BugzillaFieldsMapper(IStorageRepository storageRepository)
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

		private MappingContainer GetMappingFor(MappingSourceEntry source, IEnumerable<MappingElement> mapped)
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
			return _storageRepository.IsNull 
			       	? Enumerable.Empty<MappingElement>()
			       	: _storageRepository.GetProfile<BugzillaProfile>().StatesMapping;
		}

		private IEnumerable<MappingElement> GetMappedSeverities()
		{
			return _storageRepository.IsNull
			       	? Enumerable.Empty<MappingElement>()
					: _storageRepository.GetProfile<BugzillaProfile>().SeveritiesMapping;
		}

		private IEnumerable<MappingElement> GetMappedPriorities()
		{
			return _storageRepository.IsNull
			       	? Enumerable.Empty<MappingElement>()
					: _storageRepository.GetProfile<BugzillaProfile>().PrioritiesMapping;
		}
	}
}