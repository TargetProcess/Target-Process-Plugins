//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Xml.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Storage.Persisters.Serialization;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
	public partial class ProfileStorage : ISavingChangesEventHandler
	{
		public ProfileStorage(TypeNameWithoutVersion key)
		{
			ValueKey = key.Value;
		}

		partial void OnCreated()
		{
			GetValueInt = GetValueDeserialized;
		}

		private object GetValueCached()
		{
			return _value;
		}

		private object GetValueDeserialized()
		{
			_value = BlobSerializer.Deserialize(ValueBlob, ValueKey);
			GetValueInt = GetValueCached;
			return _value;
		}

		private Func<object> GetValueInt;

		private object _value;

		public T GetValue<T>()
		{
			return (T)GetValue();
		}

		public object GetValue()
		{
			return GetValueInt();
		}

		public void SetValue<T>(T value)
		{
			_value = value;
			GetValueInt = GetValueCached;
			ValueBlob = new XDocument();
		}

		void ISavingChangesEventHandler.OnInsert()
		{
			UpdateBlob();
		}

		void ISavingChangesEventHandler.OnDelete()
		{
		}

		void ISavingChangesEventHandler.OnUpdate()
		{
			UpdateBlob();
		}

		private void UpdateBlob()
		{
			ValueBlob = BlobSerializer.Serialize(GetValue());
		}

		public static TypeNameWithoutVersion Key(Type type)
		{
			return new TypeNameWithoutVersion(type);
		}

		public static ProfileStorage Create<T>(TypeNameWithoutVersion key, T item)
		{
			var profileStorage = new ProfileStorage(key);
			profileStorage.SetValue(item);
			return profileStorage;
		}
	}
}