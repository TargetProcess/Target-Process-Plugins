// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using StructureMap;

namespace Tp.Integration.Messages.SerializationPatches
{
	public class SerializationPatcher
	{
		public static string Apply(string text, params IPatch[] patches)
		{
			var patchCollection = new List<IPatch>(patches);
			var externalPatchCollection = ObjectFactory.TryGetInstance<IPatchCollection>();
			if (externalPatchCollection != null)
				patchCollection.AddRange(externalPatchCollection);

			foreach (var patch in patchCollection)
			{
				if (patch.NeedToApply(text))
				{
					return patch.Apply(text);
				}
			}

			return text;
		}
	}
}