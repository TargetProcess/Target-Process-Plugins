using System;

namespace Tp.Core.Features
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class MashupAttribute : Attribute
	{
		private readonly string _mashupName;
		public MashupAttribute(string mashupName)
		{
			_mashupName = mashupName;
		}

		public string MashupName
		{
			get { return _mashupName; }
		}
	}
}