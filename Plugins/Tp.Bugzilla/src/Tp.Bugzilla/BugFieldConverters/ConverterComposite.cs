// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using StructureMap;

namespace Tp.Bugzilla.BugFieldConverters
{
	public class ConverterComposite : IBugConverter
	{
		private readonly List<IBugConverter> _converters = new List<IBugConverter>
		                                                   	{
		                                                   		ObjectFactory.GetInstance<NameConverter>(),
		                                                   		ObjectFactory.GetInstance<DescriptionConverter>(),
		                                                   		ObjectFactory.GetInstance<EntityStateConverter>(),
		                                                   		ObjectFactory.GetInstance<SeverityConverter>(),
		                                                   		ObjectFactory.GetInstance<PriorityConverter>(),
		                                                   		ObjectFactory.GetInstance<CreateDateConverter>(),
		                                                   		ObjectFactory.GetInstance<CommentConverter>(),
																ObjectFactory.GetInstance<OwnerConverter>(),
		                                                   	};

		public void Apply(BugzillaBug bugzillaBug, ConvertedBug convertedBug)
		{
			foreach (var converter in _converters)
			{
				converter.Apply(bugzillaBug, convertedBug);
			}
		}
	}
}