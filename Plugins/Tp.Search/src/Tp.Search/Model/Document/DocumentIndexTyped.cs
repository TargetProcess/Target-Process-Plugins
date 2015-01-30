using System;
using System.Collections.Generic;
using System.IO;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Search.Model.Optimization;
using Tp.Search.Model.Query;
using hOOt;

namespace Tp.Search.Model.Document
{
	class DocumentIndexTyped : IDocumentIndex
	{
		private readonly DocumentIndexType _indexType;
		private readonly Hoot _hoot;
		private readonly Action _shuttedDown;
		private bool _isAlive;
		private DateTime _lastUsedTime;
		private readonly DocumentIndexSetup _documentIndexSetup;
		private readonly IDocumentIndexOptimizeHint _optimizeHint;
		private readonly IActivityLogger _logger;
		private bool _isOptimized;

		public DocumentIndexTyped(DocumentIndexType indexType, IPluginContext context, Action shuttedDown, DocumentIndexSetup documentIndexSetup, IActivityLoggerFactory loggerFactory, DocumentIndexOptimizeHintFactory optimizeHintFactory)
		{
			_indexType = indexType;
			_logger = loggerFactory.Create(new PluginContextSnapshot(context));
			var indexPath = _indexType.GetFileFolder(context.AccountName, documentIndexSetup);
			_hoot = new Hoot(indexPath, _indexType.FileName, _logger.Debug, _logger.Error, indexType.CreateTokensParser(documentIndexSetup), _logger.IsDebugEnabled);
			_shuttedDown = shuttedDown;
			_isAlive = true;
			_documentIndexSetup = documentIndexSetup;
			_optimizeHint = optimizeHintFactory.Create(documentIndexSetup);
			_isOptimized = true;
			UpdateLastUsedToken();
		}

		public List<TDocument> Find<TDocument>(QueryPlanResult plan, int page, int pageSize, int skip, out int total) where TDocument : hOOt.Document
		{
			if (!_isAlive)
			{
				total = 0;
				return new List<TDocument>();
			}
			UpdateLastUsedToken();
			return _hoot.FindPagedDocuments<TDocument>(plan.BitArray, page, pageSize, skip, out total);
		}

		public T FindDocumentByName<T>(string name) where T : hOOt.Document, new()
		{
			if (!_isAlive)
			{
				return new T();
			}
			UpdateLastUsedToken();
			return _hoot.FindDocumentByFileName<T>(name);
		}

		public IndexData GetExistingIndexByNumber(int number)
		{
			if (!_isAlive)
			{
				return new IndexData
				{
					DocNumber = -1,
					Words = new List<string>()
				};
			}
			UpdateLastUsedToken();
			return _hoot.GetExistingIndex(number);
		}

		public QueryPlan BuildExecutionPlan(ParsedQuery parsedQuery, bool freeCache)
		{
			if (!_isAlive)
			{
				return new QueryPlan(() => new WAHBitArray());
			}
			UpdateLastUsedToken();
			Func<WAHBitArray> plan = _hoot.BuildExecutionPlan(parsedQuery.Full, freeCache);
			return new QueryPlan(plan);
		}

		public DocumentIndexType Type { get { return _indexType; } }

		public bool IsAlive { get { return _isAlive; } }
		public bool IsOptimized { get { return _isOptimized; } }

		public T GetLastDocument<T>() where T : hOOt.Document
		{
			if (!_isAlive)
			{
				return default(T);
			}
			UpdateLastUsedToken();
			if (_hoot.DocumentCount <= 0)
			{
				return default(T);
			}
			return _hoot.FindDocumentByNumber<T>(_hoot.DocumentCount - 1);
		}

		public IndexResult Index(hOOt.Document document, bool deleteOld, DocumentIndexOptimizeSetup setup)
		{
			if (!_isAlive)
			{
				return new IndexResult();
			}
			UpdateLastUsedToken();
			var indexResult = _hoot.Index(document, deleteOld);
			Optimize(setup);
			return indexResult;
		}

		public void SaveDocument(hOOt.Document document, bool deleteOld)
		{
			if (!_isAlive)
			{
				return;
			}
			UpdateLastUsedToken();
			_hoot.SaveDocument(document, deleteOld);
		}

		public IndexResult Rebuild(hOOt.Document document, bool deleteOld, DocumentIndexOptimizeSetup setup)
		{
			if (!_isAlive)
			{
				return new IndexResult();
			}
			UpdateLastUsedToken();
			if (document.DocNumber > _hoot.DocumentCount - 1)
			{
				return new IndexResult();
			}
			var indexResult = _hoot.UpdateIndex(document, deleteOld);
			Optimize(setup);
			return indexResult;
		}

		public IndexResult Index(int recordNumber, string text, DocumentIndexOptimizeSetup setup)
		{
			if (!_isAlive)
			{
				return new IndexResult();
			}
			UpdateLastUsedToken();
			var indexResult = _hoot.Index(recordNumber, text);
			Optimize(setup);
			return indexResult;
		}

		public IndexResult Update(string fileName, string text, DocumentIndexOptimizeSetup setup)
		{
			if (!_isAlive)
			{
				return new IndexResult();
			}
			UpdateLastUsedToken();
			var indexResult = _hoot.UpdateIndex(fileName, text);
			Optimize(setup);
			return indexResult;
		}

		public IndexResult Update(int recordNumber, string text, DocumentIndexOptimizeSetup setup)
		{
			if (!_isAlive)
			{
				return new IndexResult();
			}
			UpdateLastUsedToken();
			var indexResult = _hoot.UpdateExistingIndex(recordNumber, text);
			Optimize(setup);
			return indexResult;
		}

		public void Optimize(DocumentIndexOptimizeSetup setup)
		{
			if (!_isAlive)
			{
				return;
			}
			_isOptimized = false;
			if (!setup.ShouldOptimize)
			{
				return;
			}
			UpdateLastUsedToken();
			if (!setup.ShouldDefer)
			{
				DoOptimize(setup.ShouldFreeMemory);
				return;
			}
			if (_optimizeHint.Advice())
			{
				DoOptimize(setup.ShouldFreeMemory);
			}
		}

		private void DoOptimize(bool freeMemory)
		{
			_hoot.OptimizeIndex(freeMemory);
			_isOptimized = true;
		}

		public bool ShouldShutdown(DocumentIndexShutdownSetup setup)
		{
			if (!_isAlive)
			{
				Log("[ShouldShutdown] index is not alive");
				return false;
			}
			if (setup.ForceShutdown)
			{
				Log("[ShouldShutdown] shutdown was forced");
				return true;
			}
			var now = DateTime.UtcNow;
			var isUsageTimeout = _lastUsedTime.AddMinutes(_documentIndexSetup.AliveTimeoutInMinutes) < now;
			Log("[ShouldShutdown] isUsageTimeout = {0}, last used time = {1}, current time = {2}".Fmt(isUsageTimeout, _lastUsedTime, now));
			return isUsageTimeout;
		}

		public bool Shutdown(DocumentIndexShutdownSetup setup)
		{
			if (!_isAlive)
			{
				return true;
			}
			if (ShouldShutdown(setup))
			{
				Log("[Shutdown] started");
				DoOptimize(true);
				try
				{
					_hoot.FreeMemory(false);
					_hoot.Shutdown();
					if (setup.CleanStorage)
					{
						string[] files = Directory.GetFiles(_hoot.Path, _hoot.FileName + ".*", SearchOption.AllDirectories);
						foreach (var file in files)
						{
							File.Delete(file);
						}
					}
					_shuttedDown();
					Log("[Shutdown] finished successfull");
					return true;
				}
				finally
				{
					_isAlive = false;
				}
			}
			return false;
		}

		private void UpdateLastUsedToken()
		{
			_lastUsedTime = DateTime.UtcNow;
		}

		private void Log(string message)
		{
			_logger.DebugFormat("[{0}]{1}", _indexType, message);
		}
	}
}
