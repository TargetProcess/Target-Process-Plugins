// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NServiceBus.Saga;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters.Serialization;

namespace Tp.Integration.Testing.Common.Persisters
{
	/// <summary>
	/// Persists sagas in memory. Use for non-production environment only.
	/// </summary>
	public class TpInMemorySagaPersister : ISagaPersister
	{
		private readonly IPluginContext _pluginContext;

		public TpInMemorySagaPersister(IPluginContext pluginContext)
		{
			_pluginContext = pluginContext;
		}

		void ISagaPersister.Complete(ISagaEntity saga)
		{
			_data.Remove(saga.Id);
		}

		T ISagaPersister.Get<T>(string property, object value)
		{
			var sagaEntities = Get<T>();

			foreach (var sagaEntity in sagaEntities)
			{
				var prop = sagaEntity.GetType().GetProperty(property);
				if (prop != null)
					if (prop.GetValue(sagaEntity, null).Equals(value))
						return sagaEntity;
			}

			return default(T);
		}

		T ISagaPersister.Get<T>(Guid sagaId)
		{
			return Get<T>().Where(x => x.Id == sagaId).FirstOrDefault();
		}

		public IEnumerable<T> Get<T>()
		{
			var sagaEntities = _data.Values.Where(x => x.ProfileName == _pluginContext.ProfileName);
			return sagaEntities.Select(x => x.DeserializeAs<ISagaEntity>()).OfType<T>();
		}


		void ISagaPersister.Save(ISagaEntity saga)
		{
			Save(saga.Id, saga);
		}

		public void Save(Guid sagaId, ISagaEntity saga)
		{
			_data[sagaId] = new ProfileSagaEntity
			                	{SagaEntity = BlobSerializer.Serialize(saga), ProfileName = _pluginContext.ProfileName};
		}

		void ISagaPersister.Update(ISagaEntity saga)
		{
			((ISagaPersister) this).Save(saga);
		}

		private readonly IDictionary<Guid, IProfileSagaEntity> _data = new Dictionary<Guid, IProfileSagaEntity>();
	}

	public interface IProfileSagaEntity
	{
		XDocument SagaEntity { get; set; }
		ProfileName ProfileName { get; set; }
		T DeserializeAs<T>();
	}

	public class ProfileSagaEntity : IProfileSagaEntity
	{
		public ProfileName ProfileName { get; set; }
		public XDocument SagaEntity { get; set; }

		public T DeserializeAs<T>()
		{
			return (T) BlobSerializer.Deserialize(SagaEntity, typeof (T).Name);
		}
	}
}