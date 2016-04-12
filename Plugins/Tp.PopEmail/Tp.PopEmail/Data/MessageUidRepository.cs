// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.PopEmailIntegration.Data
{
	public class MessageUidRepository
	{
		private readonly IStorageRepository _storageRepository;

		public MessageUidRepository(IStorageRepository storageRepository)
		{
			_storageRepository = storageRepository;
		}

		public void Add(string uid)
		{
			AddRange(new[] {uid});
		}

		public void AddRange(string[] uids)
		{
			DoAction(uids, x => x.AddRange(uids));
		}

		public void Remove(string[] skippedUids)
		{
			DoAction(skippedUids, x => x.RemoveAll(skippedUids.Contains));
		}

		private void DoAction(ICollection<string> uids, Action<MessageUidCollection> action)
		{
			if (uids.Count == 0) return;

			var uidStorage = _storageRepository.Get<MessageUidCollection>();
			var messageUidCollection = uidStorage.FirstOrDefault() ?? new MessageUidCollection();

			action(messageUidCollection);

			uidStorage.ReplaceWith(messageUidCollection);
		}

		public string[] GetUids()
		{
			return (_storageRepository.Get<MessageUidCollection>().FirstOrDefault() ?? new MessageUidCollection()).ToArray();
		}

		public void RemoveAll()
		{
			_storageRepository.Get<MessageUidCollection>().Clear();
		}
	}
}
