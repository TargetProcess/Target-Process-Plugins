// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Messages;

namespace Tp.Search.Model.Document
{
	class DocumentIndexProvider : IDocumentIndexProvider
	{
		private readonly Action<string> _logger;
		private readonly DocumentIndexSetup _documentIndexSetup;
		private readonly DocumentIndexes _documentIndexes;
		public DocumentIndexProvider(Action<string> logger, DocumentIndexSetup documentIndexSetup)
		{
			_logger = logger;
			_documentIndexSetup = documentIndexSetup;
			_documentIndexes = new DocumentIndexes();
		}

		public IEnumerable<DocumentIndexTypeToken> DocumentTypes { get { return _documentIndexes.DocumentTypes; } }

		public IDocumentIndex GetOrCreateDocumentIndex(AccountName accountName, DocumentIndexTypeToken indexTypeToken)
		{
			return DoGetOrCreateDocumentFinder(accountName, indexTypeToken);
		}

		public Maybe<IDocumentIndex> GetDocumentIndex(AccountName accountName, DocumentIndexTypeToken indexTypeToken)
		{
			return DoGetDocumentFinder(accountName, indexTypeToken);
		}

		public IEnumerable<IDocumentIndex> GetOrCreateDocumentIndexes(AccountName accountName, params DocumentIndexTypeToken[] indexTypeTokens)
		{
			return indexTypeTokens.Select(t => GetOrCreateDocumentIndex(accountName, t)).ToList();
		}

		private Maybe<IDocumentIndex> DoGetDocumentFinder(AccountName accountName, DocumentIndexTypeToken documentIndexTypeToken)
		{
			Lazy<IDocumentIndex> fetched;
			return _documentIndexes[documentIndexTypeToken].TryGetValue(accountName.Value, out fetched)
				? Maybe.Return(fetched.Value)
				: Maybe.Nothing;
		}

		private IDocumentIndex DoGetOrCreateDocumentFinder(AccountName accountName, DocumentIndexTypeToken documentIndexTypeToken)
		{
			ConcurrentDictionary<string, Lazy<IDocumentIndex>> storage = _documentIndexes[documentIndexTypeToken];
			return storage.GetOrAdd(accountName.Value, key => Lazy.Create(() =>
				{
					IDocumentIndex documentIndex = new DocumentIndex(documentIndexTypeToken, accountName, () =>
						{
							Lazy<IDocumentIndex> _;
							storage.TryRemove(accountName.Value, out _);
						}, _documentIndexSetup, _logger);
					return documentIndex;
				})).Value;
		}
	}
}
