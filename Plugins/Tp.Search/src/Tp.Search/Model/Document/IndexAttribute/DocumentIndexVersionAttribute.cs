using System;

namespace Tp.Search.Model.Document.IndexAttribute
{
	[AttributeUsage(AttributeTargets.Field)]
	class DocumentIndexVersionAttribute : Attribute
	{
		private readonly int _version;
		public DocumentIndexVersionAttribute(int version)
		{
			_version = version;
		}

		public int Version
		{
			get { return _version; }
		}
	}
}