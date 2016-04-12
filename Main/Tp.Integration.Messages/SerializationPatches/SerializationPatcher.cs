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
