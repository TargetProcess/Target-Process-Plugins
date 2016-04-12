using System;

namespace Tp.Core
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class PublicNameAttribute : Attribute
	{
		public PublicNameAttribute(string publicName)
		{
			if (publicName.IsNullOrWhitespace())
			{
				throw new ArgumentNullException(nameof(publicName));
			}
			PublicName = publicName;
		}

		public string PublicName { get; set; }
	}
}
