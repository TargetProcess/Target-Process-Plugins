// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using Tp.Integration.Common;

namespace Tp.Tfs.WorkItemsIntegration.ChangedFieldsMapping
{
	public class FeatureChangedFieldsPolicy : ChangedFieldsPolicyBase
	{
		public FeatureChangedFieldsPolicy()
		{
			FieldsMap = new Dictionary<string, Enum>
			{
				{ "Name", FeatureField.Name },
				{ "Description", FeatureField.Description }
			};
		}
	}
}
