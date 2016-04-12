using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Tp.Search.Model.Document
{
	internal class EnumDescriptions<TEnum>
	{
		private readonly IDictionary<TEnum, string> _descriptions;
		public EnumDescriptions()
		{
			var loaded = EnumServices.Load<TEnum, DescriptionAttribute>();
			_descriptions = loaded.ToDictionary(x => x.Key, x => x.Value.Description);
		}

		public string GetDescription(TEnum enumVal)
		{
			return _descriptions[enumVal];
		}
	}
}