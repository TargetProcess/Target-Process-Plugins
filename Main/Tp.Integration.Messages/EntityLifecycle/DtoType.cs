using System;

namespace Tp.Integration.Messages.EntityLifecycle
{
	[Serializable]
	public class DtoType
	{
		public DtoType()
		{
		}

		public DtoType(Type type)
		{
			FullName = type.FullName;
		}

		public new Type GetType()
		{
			return Type.GetType(FullName);
		}

		public string FullName { get; set; }
	}
}
