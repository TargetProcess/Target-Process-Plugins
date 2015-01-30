// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Search.Model.Document
{
	public class DocumentIdFactory : IDocumentIdFactory
	{
		private readonly int _int64Width = Convert.ToString(Int64.MaxValue).Length;
		private readonly int _int32Width = Convert.ToString(Int32.MaxValue).Length;
		private readonly int _int16Width = Convert.ToString(Int16.MaxValue).Length;

		public string CreateEntityTypeId(int entityTypeId)
		{
			return Convert.ToString(entityTypeId).PadLeft(_int16Width, '0');
		}

		public string CreateSquadId(int squadId)
		{
			return Convert.ToString(squadId).PadLeft(_int32Width, '0');
		}

		public string CreateEntityStateId(int entityStateId)
		{
			return Convert.ToString(entityStateId).PadLeft(_int32Width, '0');
		}

		public string CreateProjectId(int projectId)
		{
			return Convert.ToString(projectId).PadLeft(_int64Width, '0');
		}

		public int ParseProjectId(string projectId)
		{
			var trimmed = projectId.TrimStart('0');
			if (string.IsNullOrEmpty(trimmed))
			{
				return 0;
			}
			return int.Parse(trimmed);
		}
	}
}