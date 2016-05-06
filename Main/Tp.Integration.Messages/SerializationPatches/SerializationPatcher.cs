using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace Tp.Integration.Messages.SerializationPatches
{
	public class SerializationPatcher
	{
		private readonly List<IPatch> _patches;

		public SerializationPatcher(IEnumerable<IPatch> patches)
		{
			_patches = new List<IPatch>(patches);
			var externalPatchCollection = ObjectFactory.TryGetInstance<IPatchCollection>();
			if (externalPatchCollection != null)
			{
				_patches.AddRange(externalPatchCollection);
			}
		}

		public bool ShouldApply(string text)
		{
			return GetPatchesToApply(text).Any();
		}

		private IEnumerable<IPatch> GetPatchesToApply(string text)
		{
			return _patches.Where(x => x.NeedToApply(text));
		}

		public string Apply(string text)
		{
			foreach (var patch in GetPatchesToApply(text))
			{
				text = patch.Apply(text);
			}

			return text;
		}
	}
}
