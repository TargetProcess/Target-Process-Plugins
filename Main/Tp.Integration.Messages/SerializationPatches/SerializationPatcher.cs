// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;

namespace Tp.Integration.Messages.SerializationPatches
{
	public class SerializationPatcher
	{
		public static string Apply(string text)
		{
			var patchCollection = ObjectFactory.TryGetInstance<IPatchCollection>();
			if (patchCollection == null)
				return text;

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