// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.Tfs.WorkItemsIntegration
{
	[DataContract]
	public class ImportedType : ICloneable
	{
		[DataMember]
		public string Type { get; set; }

		[DataMember]
		public int StartID { get; set; }

		[DataMember]
		public bool IsFirstSync { get; set; }

		public ImportedType Clone()
		{
			return new ImportedType { Type = Type, StartID = StartID, IsFirstSync = IsFirstSync };
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}

	[DataContract]
	public class ProjectsMappingHistoryElement : MappingElement
	{
		[DataMember]
		public CreatedWorkItemsRange WorkItemsRange { get; set; }

		[DataMember]
		public bool IsCurrent { get; set; }

		[DataMember]
		public List<ImportedType> ImportedTypes { get; set; }

		public bool IsEquals(MappingElement mapping)
		{
			return Key == mapping.Key && Value.Equals(mapping.Value);
		}
	}

	public class ProjectsMappingHistory : List<ProjectsMappingHistoryElement>
	{
		public ProjectsMappingHistory()
		{
		}

		public ProjectsMappingHistory(IStorageRepository storage)
		{
			var projectsMappings = storage.Get<ProjectsMappingHistoryElement>();
			AddRange(projectsMappings);
		}

		public MappingLookup this[string key]
		{
			get
			{
				var mappingElement = Find(x => x.Key.ToLower().Trim() == key.ToLower().Trim());
				return mappingElement == null ? null : mappingElement.Value;
			}
		}

		public ProjectsMappingHistoryElement Current
		{
			get
			{
				return this.SingleOrDefault(x => x.IsCurrent);
			}
		}
	}
}
