// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.BugTracking.Commands.Dtos;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.BugTracking.Mappers
{
	public static class MappingExtensions
	{
		public static IEnumerable<MappingElement> FilterOutObsoleteItemsBy(this IEnumerable<MappingElement> mappingContainer,
		                                                                   IEnumerable<string> thirdPartyItems)
		{
			return mappingContainer.Where(s => thirdPartyItems.Contains(s.Key));
		}

		public static IEnumerable<string> FilterOutDuplicateThirdPartyItemsBy(this IEnumerable<string> thirdPartyItems,
		                                                                      IEnumerable<MappingElement> itemsToFilterOut)
		{
			return thirdPartyItems.Where(x => !itemsToFilterOut.Any(y => y.Key == x));
		}

		public static IEnumerable<MappingElement> ToMappingElements(this IEnumerable<string> items,
		                                                            MappingSourceEntry mappingSource)
		{
			return items.Select(x => new MappingElement
			                         	{
			                         		Key = x,
			                         		Value = mappingSource.TpItems
			                         			.FirstOrDefault(y => x.Equals(y.Name, StringComparison.OrdinalIgnoreCase))
			                         	});
		}

		public static IEnumerable<MappingElement> FilterOutEmptyItems(this IEnumerable<MappingElement> mappingContainer)
		{
			return mappingContainer.Where(i => !i.IsEmptyValue());
		}
	}
}