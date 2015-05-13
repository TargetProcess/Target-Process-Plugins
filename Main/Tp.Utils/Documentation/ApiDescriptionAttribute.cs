using System;

namespace Tp.Utils.Documentation
{
	[AttributeUsage(AttributeTargets.All)]
	public class ApiDescriptionAttribute : Attribute
	{
		private readonly string _description;

		public ApiDescriptionAttribute(string description)
		{
			_description = description;
		}

		public string Description
		{
			get { return _description; }
		}
	}
}