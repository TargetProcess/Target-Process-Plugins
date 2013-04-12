// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Text;

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

		public string EncodeProjectId(int? projectId)
		{
			return EncodeStringId(projectId, "Project");
		}

		public string EncodeEntityStateId(int entityStateId)
		{
			return EncodeStringId(entityStateId, "Entitystate");
		}

		public string EncodeSquadId(int? squadId)
		{
			return EncodeStringId(squadId, "Squad");
		}

		private static string EncodeStringId(int? id, string prefix)
		{
			if (id == null)
			{
				return prefix + "null";
			}
			var array = new BitArray(new[] {id.Value});
			var b = new StringBuilder();
			foreach (bool f in array)
			{
				b.Append(f ? "t" : "f");
			}
			return prefix + b;
		}
	}
}