// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Optimization;

namespace Tp.Search.Model.Document
{
	class DocumentIndexProvider : IDocumentIndexProvider
	{
		private readonly IActivityLoggerFactory _loggerFactory;
		private readonly DocumentIndexSetup _documentIndexSetup;
		private readonly DocumentIndexMetadata _documentIndexMetadata;
		private readonly DocumentIndexOptimizeHintFactory _optimizeHintFactory;
		private readonly DocumentIndexes _documentIndexes;
		public DocumentIndexProvider(IActivityLoggerFactory loggerFactory, DocumentIndexSetup documentIndexSetup, DocumentIndexMetadata documentIndexMetadata, DocumentIndexOptimizeHintFactory optimizeHintFactory)
		{
			_loggerFactory = loggerFactory;
			_documentIndexSetup = documentIndexSetup;
			_documentIndexMetadata = documentIndexMetadata;
			_optimizeHintFactory = optimizeHintFactory;
			_documentIndexes = new DocumentIndexes();
		}

		public IEnumerable<DocumentIndexType> DocumentIndexTypes { get { return _documentIndexMetadata.DocumentIndexTypes; } }

		public IDocumentIndex GetOrCreateDocumentIndex(IPluginContext context, DocumentIndexTypeToken indexTypeToken)
		{
			return DoGetOrCreateDocumentIndex(context, indexTypeToken);
		}

		public Maybe<IDocumentIndex> GetDocumentIndex(IPluginContext context, DocumentIndexTypeToken indexTypeToken)
		{
			return DoGetDocumentIndex(context, indexTypeToken);
		}

		public IEnumerable<IDocumentIndex> GetOrCreateDocumentIndexes(IPluginContext context, params DocumentIndexTypeToken[] indexTypeTokens)
		{
			return indexTypeTokens.Select(t => GetOrCreateDocumentIndex(context, t)).ToList();
		}

		private Maybe<IDocumentIndex> DoGetDocumentIndex(IPluginContext context, DocumentIndexTypeToken documentIndexTypeToken)
		{
			Lazy<IDocumentIndex> fetched;
			return _documentIndexes[documentIndexTypeToken].TryGetValue(context.AccountName.Value, out fetched)
				? Maybe.Return(fetched.Value)
				: Maybe.Nothing;
		}

		private IDocumentIndex DoGetOrCreateDocumentIndex(IPluginContext context, DocumentIndexTypeToken documentIndexTypeToken)
		{
			ConcurrentDictionary<string, Lazy<IDocumentIndex>> storage = _documentIndexes[documentIndexTypeToken];
			var index = storage.GetOrAdd(context.AccountName.Value, key => Lazy.Create(() =>
				{
					IDocumentIndex documentIndex = CreateDocumentIndex(context, documentIndexTypeToken);
					var otherVersions = documentIndex.Type.GetVersions(context.AccountName, _documentIndexSetup).Except(new[] {documentIndex.Type.Version});
					foreach (int version in otherVersions)
					{
						IDocumentIndex versionedDocumentIndex = CreateDocumentIndex(context, documentIndexTypeToken, version);
						versionedDocumentIndex.Shutdown(new DocumentIndexShutdownSetup(forceShutdown: true, cleanStorage: true));
					}
					return documentIndex;
				}));
			return index.Value;
		}

		private IDocumentIndex CreateDocumentIndex(IPluginContext context, DocumentIndexTypeToken documentIndexTypeToken, int? version = null)
		{
			DocumentIndexType charactersIndexType = null;
			DocumentIndexType digitsIndexType = null;
			if (_documentIndexMetadata.Contains(documentIndexTypeToken, DocumentIndexDataTypeToken.Characters))
			{
				charactersIndexType = _documentIndexMetadata.GetDocumentIndexType(documentIndexTypeToken, DocumentIndexDataTypeToken.Characters);
			}
			if (_documentIndexMetadata.Contains(documentIndexTypeToken, DocumentIndexDataTypeToken.Digits))
			{
				digitsIndexType = _documentIndexMetadata.GetDocumentIndexType(documentIndexTypeToken, DocumentIndexDataTypeToken.Digits);
			}
			if (version != null)
			{
				if (charactersIndexType != null)
				{
					charactersIndexType = charactersIndexType.CreateVersion(version.Value);
				}
				if (digitsIndexType != null)
				{
					digitsIndexType = digitsIndexType.CreateVersion(version.Value);
				}
			}
			DocumentIndexMonitor monitor = null;
			Action detachIndex = () =>
				{
					if (monitor != null)
					{
						monitor.DetachIndex();
					}
				};
			IActivityLogger logger = _loggerFactory.Create(new PluginContextSnapshot(context));
			monitor = new DocumentIndexMonitor(charactersIndexType ?? digitsIndexType, () =>
				{
					if (charactersIndexType != null)
					{
						return digitsIndexType != null
								   ? (IDocumentIndex)new DocumentIndex(charactersIndexType, digitsIndexType, context, detachIndex, _documentIndexSetup, _loggerFactory, _optimizeHintFactory)
								   : new DocumentIndexTyped(charactersIndexType, context, detachIndex, _documentIndexSetup, _loggerFactory, _optimizeHintFactory);
					}
					throw new ApplicationException("Characters index for {0} type is not supported.".Fmt(documentIndexTypeToken));
				}, logger);
			return monitor;
		}
	}
}
