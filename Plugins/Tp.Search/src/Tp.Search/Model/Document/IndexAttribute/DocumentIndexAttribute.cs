using System;
using System.Collections.Generic;

namespace Tp.Search.Model.Document.IndexAttribute
{
	[AttributeUsage(AttributeTargets.Class)]
	class DocumentIndexAttribute : Attribute
	{
		private readonly Type[] _indexAttributeTypes;
		private readonly Type[] _documentAttributeTypes;
		private readonly Type _versionAttributeType;
		private readonly Type _dataTypeTokenAttributeType;

		public DocumentIndexAttribute(Type[] indexAttributeTypes, Type[] documentAttributeTypes, Type versionAttributeType, Type dataTypeTokenAttributeType)
		{
			_indexAttributeTypes = indexAttributeTypes;
			_documentAttributeTypes = documentAttributeTypes;
			_versionAttributeType = versionAttributeType;
			_dataTypeTokenAttributeType = dataTypeTokenAttributeType;
		}

		public IEnumerable<Type> IndexAttributeTypes
		{
			get { return _indexAttributeTypes; }
		}

		public Type[] DocumentAttributeTypes
		{
			get { return _documentAttributeTypes; }
		}

		public Type VersionAttributeType
		{
			get { return _versionAttributeType; }
		}

		public Type DataTypeTokenAttributeType
		{
			get { return _dataTypeTokenAttributeType; }
		}
	}
}