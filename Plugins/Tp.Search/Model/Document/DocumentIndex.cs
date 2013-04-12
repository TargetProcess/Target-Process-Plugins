using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Tp.Integration.Messages;
using hOOt;

namespace Tp.Search.Model.Document
{
	class DocumentIndex : IDocumentIndex
	{
		private readonly DocumentIndexType _documentType;
		private readonly Hoot _hoot;
		private readonly Action _shuttingDown;
		private bool _isAlive;
		private readonly object _gate;
		private DateTime _lastUsedTime;
		private readonly DocumentIndexSetup _documentIndexSetup;
		private static readonly string BasePath;
		private static readonly IDictionary<DocumentIndexTypeToken, DocumentIndexType> DocumentIndexTypes;
		static DocumentIndex()
		{
			BasePath = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
			DocumentIndexTypes = DocumentIndexType.Load();
		}


		public DocumentIndex(DocumentIndexTypeToken documentIndexTypeToken, AccountName accountName, Action shuttingDown, DocumentIndexSetup documentIndexSetup, Action<string> log)
		{
			_documentType = DocumentIndexTypes[documentIndexTypeToken];
			_hoot = new Hoot(GetIndexPath(documentIndexSetup, accountName), _documentType.FileName, log, documentIndexSetup.MinStringLengthToSearch, documentIndexSetup.MaxStringLengthIgnore);
			_shuttingDown = shuttingDown;
			_isAlive = true;
			_gate = new object();
			_documentIndexSetup = documentIndexSetup;
			UpdateToken();
		}

		public List<TDocument> Find<TDocument>(Lazy<WAHBitArray> plan, int page, int pageSize, int skip, out int total) where TDocument : hOOt.Document
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					total = 0;
					return new List<TDocument>();
				}
				UpdateToken();
				return _hoot.FindPagedDocuments<TDocument>(plan, page, pageSize, skip, out total);
			}
		}

		public T FindDocumentByName<T>(string name) where T : hOOt.Document, new()
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return new T();
				}
				UpdateToken();
				return _hoot.FindDocumentByFileName<T>(name);
			}
		}
		  
		public Lazy<WAHBitArray> BuildExecutionPlan(string query, bool freeCache)
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return new Lazy<WAHBitArray>(() => new WAHBitArray());
				}
				UpdateToken();
				return _hoot.BuildExecutionPlan(query, freeCache);
			}
		}

		public DocumentIndexType Type { get { return _documentType; } }
		public T GetLastDocument<T>() where T : hOOt.Document
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return default(T);
				}
				UpdateToken();
				if (_hoot.DocumentCount <= 0)
				{
					return default(T);
				}
				return _hoot.FindDocumentByNumber<T>(_hoot.DocumentCount - 1);
			}
		}

		public IndexResult Index(hOOt.Document document, bool deleteOld/*, bool optimize*/)
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return new IndexResult();
				}
				UpdateToken();
				return _hoot.Index(document, deleteOld/*, optimize*/);
			}
		}

		public void SaveDocument(hOOt.Document document, bool deleteOld)
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return;
				}
				UpdateToken();
				_hoot.SaveDocument(document, deleteOld);
			}
		}

		public IndexResult Rebuild(hOOt.Document document, bool deleteOld = false)
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return new IndexResult();
				}
				UpdateToken();
				return document.DocNumber > _hoot.DocumentCount - 1 ? new IndexResult() : _hoot.UpdateIndex(document, deleteOld);
			}
		}

		public IndexResult Index(int recordNumber, string text)
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return new IndexResult();
				}
				UpdateToken();
				return _hoot.Index(recordNumber, text);
			}
		}

		public IndexResult Update(string fileName, string text)
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return new IndexResult();
				}
				UpdateToken();
				return _hoot.UpdateIndex(fileName, text);
			}
		}

		public IndexResult Update(int recordNumber, string text)
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return new IndexResult();
				}
				UpdateToken();
				return _hoot.UpdateExistingIndex(recordNumber, text);
			}
		}

		public void Optimize(bool freeMemory)
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return;
				}
				UpdateToken();
				_hoot.OptimizeIndex(freeMemory);
			}
		}
		
		public bool Shutdown(DocumentIndexShutdownSetup setup)
		{
			lock (_gate)
			{
				if (!_isAlive)
				{
					return true;
				}
				if (setup.ForceShutdown || _lastUsedTime.AddMinutes(_documentIndexSetup.AliveTimeoutInMinutes) < DateTime.UtcNow)
				{
					try
					{
						_shuttingDown();
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
						return true;
					}
					finally
					{
						_isAlive = false;
					}
				}
			}
			return false;
		}
		
		private void UpdateToken()
		{
			_lastUsedTime = DateTime.UtcNow;
		}

		private string GetIndexPath(DocumentIndexSetup documentIndexSetup, AccountName accountName)
		{
			string basePath = string.IsNullOrEmpty(documentIndexSetup.IndexPath) ? string.Format("{0}\\{1}", BasePath, "index") : documentIndexSetup.IndexPath;
			var isOnSite = accountName == AccountName.Empty;
			return Path.Combine(basePath, isOnSite ? basePath : string.Format("{0}\\{1}", basePath, accountName.Value));
		}
	}
}