// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using fastJSON;

namespace hOOt
{
	public class Hoot
	{
		public Hoot(string indexPath, string fileName, Action<string> log, Action<string> logError, ITokensParser tokensParser, bool detailedTrace = false)
		{
			_path = indexPath;
			_fileName = fileName;
			_log = log;
			_logError = logError;
			_tokensParser = tokensParser;
			_detailedTrace = detailedTrace;
			if (_path.EndsWith("\\") == false)
			{
				_path += "\\";
			}
			Directory.CreateDirectory(indexPath);
			Log("\r\n\r\n");
			Log("Starting....");
			Log(string.Format("Storage Folder = {0}{1}", _path, _fileName));
			_docs = new StorageFile(_path + _fileName + ".docs", 4);
			_hash = new Hash(_path + _fileName + ".idx", 255, 10, 1009);
			_lastDocNum = (int) _hash.Count();
			LoadWords();
			ReadDeleted();
			var bitmapPath = _path + _fileName + ".bitmap";
			_bitmapFile = new FileStream(bitmapPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
			_lastBitmapOffset = _bitmapFile.Seek(0L, SeekOrigin.End);
		}

		public string Path
		{
			get { return _path; }
		}

		public string FileName
		{
			get { return _fileName; }
		}

		private readonly Hash _hash;
		private int _lastDocNum;
		private readonly string _fileName = "words";
		private readonly Action<string> _log;
		private readonly Action<string> _logError;
		private readonly ITokensParser _tokensParser;
		private readonly bool _detailedTrace;
		private readonly string _path = "";
		private readonly Dictionary<string, Cache> _index = new Dictionary<string, Cache>(50000);
		private WAHBitArray _deleted = new WAHBitArray();
		private readonly StorageFile _docs;
		private bool _internalOP;
		private readonly object _lock = new object();
		private FileStream _bitmapFile;
		private long _lastBitmapOffset;

		public int WordCount()
		{
			return _index.Count;
		}

		public int DocumentCount
		{
			get { return _lastDocNum; }
		}

		public void FreeMemory(bool freecache)
		{
			Log("FreeMemory started");
			LogIndex();
			lock (_lock)
			{
				_internalOP = true;
				Log("freeing memory");
				_deleted.FreeMemory();
				_hash.SaveIndex();
				foreach (Cache c in _index.Values)
				{
					if (freecache)
					{
						long off = SaveBitmap(c.GetCompressedBits(), c.LastBitSaveLength, c.FileOffset);
						c.isDirty = false;
						c.FileOffset = off;
						c.FreeMemory(true);
					}
					else
						c.FreeMemory(false);
				}
				_internalOP = false;
			}
			LogIndex();
			Log("FreeMemory ended");
		}

		//public void Save(bool freeCache)
		//{
		//	lock (_lock)
		//	{
		//		_internalOP = true;
		//		InternalSave(freeCache);
		//		_internalOP = false;
		//	}
		//}

		public void Shutdown()
		{
			lock (_lock)
			{
				_docs.Shutdown();
				_hash.Shutdown();
				_bitmapFile.Flush();
				_bitmapFile.Close();
			}
		}

		public IndexResult Index(int recordnumber, string text)
		{
			while (_internalOP) Thread.Sleep(50);

			return AddtoIndex(recordnumber, text);
		}

		public IndexResult Index(Document doc, bool deleteold)
		{
			while (_internalOP) Thread.Sleep(50);
			SaveDocument(doc, deleteold);
			DateTime dt = FastDateTime.Now;
			var result = AddtoIndex(doc.DocNumber, doc.Text);
			Log(string.Format("indexing time (ms) = {0}", FastDateTime.Now.Subtract(dt).TotalMilliseconds));
			return result;
		}

		public IndexResult UpdateIndex(string fileName, string text)
		{
			while (_internalOP) Thread.Sleep(50);

			if (_hash.Contains(Helper.GetBytes(fileName)))
			{
				int offset;
				_hash.Get(Helper.GetBytes(fileName), out offset);
				return UpdateExistingIndex(offset, text);
			}
			return new IndexResult();
		}

		public IndexResult UpdateIndex(Document doc, bool deleteold)
		{
			Log("UpdateIndex started");
			LogIndex();
			while (_internalOP) Thread.Sleep(50);

			SaveDocument(doc, deleteold);

			if (deleteold)
			{
				return new IndexResult();
			}
			DateTime dt = FastDateTime.Now;
			var result = UpdateExistingIndex(doc.DocNumber, doc.Text);
			Log(string.Format("indexing time (ms) = {0}", FastDateTime.Now.Subtract(dt).TotalMilliseconds));
			LogIndex();
			Log("UpdateIndex ended");
			return result;
		}

		public void SaveDocument(Document doc, bool deleteold)
		{
			while (_internalOP) Thread.Sleep(50);

			DateTime dt = FastDateTime.Now;
			if (deleteold && doc.DocNumber > -1)
			{
				_deleted.Set(doc.DocNumber, true);
			}
			if (deleteold || doc.DocNumber == -1)
			{
				doc.DocNumber = _lastDocNum++;
			}
			Log(string.Format("indexing document: #{0} for Entity #{1}", doc.DocNumber, doc.FileName));
			string dstr = JSON.Instance.ToJSON(doc, false);
			_docs.WriteData(Helper.GetBytes(doc.DocNumber, false), Helper.GetBytes(dstr));
			_hash.Set(Helper.GetBytes(doc.FileName), doc.DocNumber);
			Log(string.Format("writing document to disk (ms) = {0}", FastDateTime.Now.Subtract(dt).TotalMilliseconds));
		}

		public IEnumerable<int> FindRows(string filter)
		{
			int total;
			return FindPagedRows(filter, -1, -1, 0, out total);
		}

		public List<int> FindPagedRows(string filter, int page, int pageSize, int skip, out int total)
		{
			var result = new List<int>();
			while (_internalOP) Thread.Sleep(50);

			WAHBitArray bits = ExecutionPlan(filter);
			var indexes = bits.GetBitIndexes().ToList();
			total = indexes.Count;
			int currentPos = 0, pageStartPos = page*pageSize + (skip > 0 ? skip : 0), pageEndPos = pageStartPos + pageSize;
			bool isPageable = pageStartPos < pageEndPos;
			if (!isPageable)
			{
				return indexes;
			}
			foreach (int i in indexes)
			{
				currentPos++;
				if (currentPos <= pageStartPos || currentPos > pageEndPos) continue;
				result.Add(i);
				if (result.Count == pageSize) break;
			}
			return result;
		}

		public TDocument FindDocumentByNumber<TDocument>(int docNumber) where TDocument : Document
		{
			while (_internalOP) Thread.Sleep(50);

			return docNumber > _lastDocNum - 1
				       ? default(TDocument)
				       : JSON.Instance.ToObject<TDocument>(Helper.GetString(_docs.ReadData(docNumber)));
		}

		public TDocument FindDocumentByFileName<TDocument>(string fileName) where TDocument : Document
		{
			while (_internalOP) Thread.Sleep(50);

			var bytes = Helper.GetBytes(fileName);
			if (_hash.Contains(bytes))
			{
				int offset;
				_hash.Get(bytes, out offset);
				return JSON.Instance.ToObject<TDocument>(Helper.GetString(_docs.ReadData(offset)));
			}
			return default(TDocument);
		}

		public List<TDocument> FindPagedDocuments<TDocument>(WAHBitArray plan, int page, int pageSize, int skip, out int total)
			where TDocument : Document
		{
			var result = new List<TDocument>();
			int pageStartPos = page*pageSize + (skip > 0 ? skip : 0), pageEndPos = pageStartPos + pageSize;
			int[] indexes = plan.GetBitIndexes().ToArray();
			total = indexes.Length;
			for (int k = total - pageStartPos - 1; k > total - pageEndPos - 1; k--)
			{
				if (k < 0)
				{
					break;
				}
				var index = indexes[k];
				if (index > _lastDocNum - 1)
				{
					continue;
				}
				byte[] b = _docs.ReadData(index);
				var d = JSON.Instance.ToObject<TDocument>(Helper.GetString(b));
				result.Add(d);
			}
			return result;
		}

		public void RemoveDocument(int number)
		{
			while (_internalOP) Thread.Sleep(50);
			// add number to deleted bitmap
			_deleted.Set(number, true);
		}

		public void OptimizeIndex(bool freeCache)
		{
			Log("OptimizeIndex started");
			LogIndex();
			lock (_lock)
			{
				_internalOP = true;
				InternalSave(freeCache);
				//_log.Debug("optimizing index..");
				Log("optimizing index..");
				DateTime dt = FastDateTime.Now;
				_lastBitmapOffset = 0;
				_bitmapFile.Flush();
				_bitmapFile.Close();
				// compact bitmap index file to new file
				_bitmapFile = new FileStream(_path + _fileName + ".bitmap$", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				MemoryStream ms = new MemoryStream();
				BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);
				// save words and bitmaps
				using (FileStream words = new FileStream(_path + _fileName + ".words", FileMode.Create))
				{
					foreach (KeyValuePair<string, Cache> kv in _index)
					{
						bw.Write(kv.Key);
						uint[] ar = LoadBitmap(kv.Value.FileOffset);
						long offset = SaveBitmap(ar, ar.Length, 0);
						kv.Value.FileOffset = offset;
						bw.Write(kv.Value.FileOffset);
					}
					// save words
					byte[] b = ms.ToArray();
					words.Write(b, 0, b.Length);
					words.Flush();
					words.Close();
				}
				// rename files
				_bitmapFile.Flush();
				_bitmapFile.Close();
				try
				{
					File.Delete(_path + _fileName + ".bitmap");
				}
				catch (IOException exception)
				{
					_logError(exception.ToString());
					throw;
				}
				File.Move(_path + _fileName + ".bitmap$", _path + _fileName + ".bitmap");
				// reload everything
				_bitmapFile = new FileStream(_path + _fileName + ".bitmap", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				_lastBitmapOffset = _bitmapFile.Seek(0L, SeekOrigin.End);
				Log(string.Format("optimizing index done = {0} sec", DateTime.Now.Subtract(dt).TotalSeconds));
				_internalOP = false;
			}
			LogIndex();
			Log("OptimizeIndex ended");
		}

		#region [  P R I V A T E   M E T H O D S  ]

		public Func<WAHBitArray> BuildExecutionPlan(string filter, bool freeCache)
		{
			return () => ExecutionPlan(filter, freeCache);
		}

		public WAHBitArray ExecutionPlan(string filter, bool freeCache = false)
		{
			Log("ExecutionPlan started");
			Log(string.Format("freeCache is {0}", freeCache));
			Log(string.Format("query : {0}", filter));
			LogIndex();
			DateTime now = FastDateTime.Now;
			string[] words = filter.Split(' ').OrderByDescending(w => w.Length).ToArray();
			WAHBitArray bits = null;
			var wildcardMatchers = new List<WildcardMatcher>();
			var cacheToFree = new List<Tuple<string,Cache>>();
			for (int i = 0; i < words.Length; i++)
			{
				string originWord = words[i];
				string preparedWord = words[i];
				var op = Cache.OPERATION.OR;
				if (originWord.StartsWith("+"))
				{
					op = Cache.OPERATION.AND;
					preparedWord = originWord.Replace("+", "");
				}
				else if (originWord.StartsWith("-"))
				{
					op = Cache.OPERATION.ANDNOT;
					preparedWord = originWord.Replace("-", "");
				}
				if (originWord.Contains("*") || originWord.Contains("?"))
				{
					wildcardMatchers.Add(new WildcardMatcher(originWord.ToLower()));
				}
				else
				{
					Cache c;
					var lowerWord = preparedWord.ToLowerInvariant();
					if (_index.TryGetValue(lowerWord, out c))
					{
						LoadCacheIfNotLoaded(c);
						cacheToFree.Add(Tuple.Create(lowerWord, c));
						bits = DoBitOperation(bits, c, op);
					}
					else if (op == Cache.OPERATION.AND)
					{
						var cache = new Cache { isLoaded = true };
						cache.SetBit(0, false);
						bits = DoBitOperation(bits, cache, op);
					}
				}
				if (i == words.Length - 1)
				{
					//asc: brutal hack - only for wildcards
					op = Cache.OPERATION.AND;
					WAHBitArray wildbits = null;
					var foundMatcherWords = wildcardMatchers.ToDictionary(w => w, _ => new ConcurrentQueue<string>());
					Parallel.ForEach(_index.Keys, w =>
						{
							foreach (var matcher in wildcardMatchers)
							{
								if (matcher.IsMatch(w))
								{
									foundMatcherWords[matcher].Enqueue(w);
									break;
								}
							}
						});
					var loadWatch = Stopwatch.StartNew();
					using (var bmp = CreateBitmapStream())
					{
						foreach (string key in foundMatcherWords.Values.SelectMany(_ => _))
						{
							var c = _index[key];
							LoadCacheIfNotLoaded(c, bmp);
							cacheToFree.Add(Tuple.Create(key, c));
						}
					}
					Log(string.Format("wildcard load operation: {0} ms", loadWatch.Elapsed.TotalMilliseconds));
					var bitWatch = Stopwatch.StartNew();
					var matcherPlans = foundMatcherWords.Select(p =>
					{
						if (p.Value.Count == 0)
						{
							var falsePlan = new WAHBitArray();
							falsePlan.Set(0, false);
							return falsePlan;
						}
						WAHBitArray matcherPlan = null;
						foreach (string word in p.Value)
						{
							matcherPlan = DoBitOperation(matcherPlan, _index[word], Cache.OPERATION.OR);
						}
						return matcherPlan;
					}).Where(p => p != null).ToList();
					wildbits = matcherPlans.Aggregate(wildbits, (acc, matcherPlan) => acc != null ? acc.And(matcherPlan) : matcherPlan);
					Log(string.Format("wildcard bit operation: {0} ms", bitWatch.Elapsed.TotalMilliseconds));
					if (wildbits != null)
					{
						bits = bits == null ? wildbits : (op == Cache.OPERATION.AND ? bits.And(wildbits) : bits.Or(wildbits));
					}
				}
			}
			if (bits == null)
			{
				return new WAHBitArray();
			}
			// remove deleted docs
			if (bits.Length > _deleted.Length)
			{
				_deleted.Length = bits.Length;
			}
			else if (bits.Length < _deleted.Length)
			{
				bits.Length = _deleted.Length;
			}
			WAHBitArray result = bits.And(_deleted.Not());
			Log(string.Format("ExecutionPlan freeCache is {0}", freeCache));
			if (freeCache)
			{
				foreach (var c in cacheToFree.Where(c => !c.Item2.isDirty))
				{
					Log(string.Format("Free cache from ExecutionPlan for {0}", c.Item1));
					c.Item2.FreeMemory(unload: true, freeUncompressedMemory: false);
				}
			}
			Log(string.Format("query time (ms) = {0}", FastDateTime.Now.Subtract(now).TotalMilliseconds));
			Log("ExecutionPlan ended");
			return result;
		}

		private static WAHBitArray DoBitOperation(WAHBitArray bits, Cache c, Cache.OPERATION op)
		{
			if (bits != null)
			{
				bits = c.Op(bits, op);
			}
			else
			{
				bits = c.GetBitmap();
				bits = op == Cache.OPERATION.ANDNOT ? bits.Not() : bits;
			}
			return bits;
		}

		private void LoadCacheIfNotLoaded(Cache c, FileStream bmp = null)
		{
			if (!c.isLoaded)
			{
				if (c.FileOffset != -1)
				{
					uint[] bits = bmp != null ? LoadBitmap(bmp, c.FileOffset) : LoadBitmap(c.FileOffset);
					c.SetCompressedBits(bits);
				}
				else
				{
					c.SetCompressedBits(new uint[] {0});
				}
			}
		}

		private void InternalSave(bool freeCache)
		{
			//_log.Debug("saving index...");
			Log("saving index...");
			DateTime dt = FastDateTime.Now;
			// save deleted
			WriteDeleted();
			// save hash index
			_hash.SaveIndex();

			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);

			// save words and bitmaps
			using (FileStream words = new FileStream(_path + _fileName + ".words", FileMode.Create))
			{
				foreach (KeyValuePair<string, Cache> kv in _index)
				{
					bw.Write(kv.Key);
					if (kv.Value.isDirty)
					{
						// write bit index
						uint[] ar = kv.Value.GetCompressedBits();
						if (ar != null)
						{
							// save bitmap data to disk
							long off = SaveBitmap(ar, kv.Value.LastBitSaveLength, kv.Value.FileOffset);
							// set the saved info in cache
							kv.Value.FileOffset = off;
							kv.Value.LastBitSaveLength = ar.Length;
							// set the word bitmap offset
							bw.Write(kv.Value.FileOffset);
						}
						else
							bw.Write(kv.Value.FileOffset);
					}
					else
						bw.Write(kv.Value.FileOffset);
					if (freeCache)
					{
						kv.Value.FreeMemory(unload: true, freeUncompressedMemory: false);
					}
					kv.Value.isDirty = false;
				}
				byte[] b = ms.ToArray();
				words.Write(b, 0, b.Length);
				words.Flush();
				words.Close();
			}
			//_log.Debug("save time (ms) = " + FastDateTime.Now.Subtract(dt).TotalMilliseconds);
			Log(string.Format("save time (ms) = {0}", FastDateTime.Now.Subtract(dt).TotalMilliseconds));
		}

		private void ReadDeleted()
		{
			if (File.Exists(_path + _fileName + ".deleted") == false)
			{
				_deleted = new WAHBitArray();
				return;
			}
			using (
				FileStream del = new FileStream(_path + _fileName + ".deleted", FileMode.Open, FileAccess.ReadWrite,
				                                FileShare.ReadWrite))
			{
				List<uint> ar = new List<uint>();
				byte[] b = new byte[4];
				while (del.Read(b, 0, 4) > 0)
				{
					ar.Add((uint) Helper.ToInt32(b, 0));
				}
				_deleted = new WAHBitArray(WAHBitArray.TYPE.Compressed_WAH, ar.ToArray());

				del.Close();
			}
		}

		private void WriteDeleted()
		{
			using (
				FileStream del = new FileStream(_path + _fileName + ".deleted", FileMode.Create, FileAccess.ReadWrite,
				                                FileShare.ReadWrite))
			{
				uint[] b = _deleted.GetCompressed();

				foreach (uint i in b)
				{
					del.Write(Helper.GetBytes((int) i, false), 0, 4);
				}
				del.Flush();
				del.Close();
			}
		}

		private void LoadWords()
		{
			if (File.Exists(_path + _fileName + ".words") == false)
			{
				return;
			}
			using (var reader = new BinaryReader(File.Open(_path + _fileName + ".words", FileMode.Open), Encoding.UTF8))
			{
				while (reader.BaseStream.Position != reader.BaseStream.Length)
				{
					string word = reader.ReadString();
					long offset = reader.ReadInt64();
					var c = new Cache { isLoaded = false, isDirty = false, FileOffset = offset};
					_index.Add(word, c);
				}
				reader.Close();
			}
			Log("Word Count = " + _index.Count);
		}

		//-----------------------------------------------------------------
		// BITMAP FILE FORMAT
		//    0  'B','M'
		//    2  uint count = 4 bytes
		//    6  '0'
		//    7  uint data
		//-----------------------------------------------------------------
		private long SaveBitmap(uint[] bits, int lastsize, long offset)
		{
			long off = _lastBitmapOffset;

			byte[] b = new byte[bits.Length*4 + 7];
			// write header data
			b[0] = ((byte) 'B');
			b[1] = ((byte) 'M');
			Buffer.BlockCopy(Helper.GetBytes(bits.Length, false), 0, b, 2, 4);
			b[6] = (0);

			for (int i = 0; i < bits.Length; i++)
			{
				byte[] u = Helper.GetBytes((int) bits[i], false);
				Buffer.BlockCopy(u, 0, b, i*4 + 7, 4);
			}
			_bitmapFile.Write(b, 0, b.Length);
			_lastBitmapOffset += b.Length;
			_bitmapFile.Flush();
			return off;
		}

		private uint[] LoadBitmap(long offset)
		{
			if (offset == -1)
			{
				return null;
			}
			using (var bmp = CreateBitmapStream())
			{
				return LoadBitmap(bmp, offset);
			}
		}

		private FileStream CreateBitmapStream()
		{
			return new FileStream(_path + _fileName + ".bitmap", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}

		private uint[] LoadBitmap(FileStream bmp, long offset)
		{
			if (offset == -1)
			{
				return null;
			}
			var ar = new List<uint>();
			bmp.Seek(offset, SeekOrigin.Begin);
			byte[] b = new byte[7];
			bmp.Read(b, 0, 7);
			if (b[0] == (byte) 'B' && b[1] == (byte) 'M' && b[6] == 0)
			{
				int c = Helper.ToInt32(b, 2);
				for (int i = 0; i < c; i++)
				{
					bmp.Read(b, 0, 4);
					ar.Add((uint) Helper.ToInt32(b, 0));
				}
			}
			return ar.ToArray();
		}

		private IndexResult AddtoIndex(int recnum, string text/*, bool optimizeIndexes*/)
		{
			LogIndex();
			Log("AddToIndex started");
			Log("text size = " + text.Length);
			Dictionary<string, int> wordFrequences = GenerateWordFreq(text);
			Log("word count = " + wordFrequences.Count);
			var result = new IndexResult {DocNumber = recnum};
			using (var bmp = CreateBitmapStream())
			{
				foreach (string word in wordFrequences.Keys)
				{
					Cache cache;
					if (_index.TryGetValue(word, out cache))
					{
						LoadCacheIfNotLoaded(cache, bmp);
						cache.SetBit(recnum, true);
					}
					else
					{
						cache = new Cache {isLoaded = true};
						cache.SetBit(recnum, true);
						_index.Add(word, cache);
					}
					result.WordsAdded.Add(word, wordFrequences[word]);
				}
			}
			LogIndex();
			Log("AddToIndex ended");
			return result;
		}

		public IndexData GetExistingIndex(int recnum)
		{
			LogIndex();
			using (var bmp = CreateBitmapStream())
			{
				var words = new List<string>();
				foreach (KeyValuePair<string, Cache> vc in _index)
				{
					string indexedWord = vc.Key;
					Cache cache = vc.Value;
					LoadCacheIfNotLoaded(cache, bmp);
					bool isWordIndexed = cache.GetBitmap().Get(recnum);
					if (isWordIndexed)
					{
						words.Add(indexedWord);
					}
				}
				return new IndexData
				{
					DocNumber = recnum,
					Words = words
				};
			}
		}

		public IndexResult UpdateExistingIndex(int recnum, string text)
		{
			Log("UpdateExistingIndex started");
			LogIndex();
			Log(string.Format("text size = {0}", text.Length));
			Dictionary<string, int> wordFrequencies = GenerateWordFreq(text);
			Log(string.Format("word count = {0}", wordFrequencies.Count));
			var result = new IndexResult {DocNumber = recnum};
			using (var bmp = CreateBitmapStream())
			{
				foreach (KeyValuePair<string, Cache> vc in _index)
				{
					string indexedWord = vc.Key;
					Cache cache = vc.Value;
					LoadCacheIfNotLoaded(cache, bmp);
					bool isWordIndexed = cache.GetBitmap().Get(recnum);
					bool isWordInText = wordFrequencies.ContainsKey(indexedWord);
					if (isWordIndexed && isWordInText)
					{
						wordFrequencies.Remove(indexedWord);
					}
					else if (isWordIndexed)
					{
						result.WordsRemoved.Add(indexedWord);
						cache.SetBit(recnum, false);
					}
					else if (isWordInText)
					{
						result.WordsAdded.Add(indexedWord, wordFrequencies[indexedWord]);
						wordFrequencies.Remove(indexedWord);
						cache.SetBit(recnum, true);
					}
				}
			}
			foreach (var wordFrequency in wordFrequencies)
			{
				result.WordsAdded.Add(wordFrequency.Key, wordFrequency.Value);
				var cache = new Cache { isLoaded = true};
				cache.SetBit(recnum, true);
				_index.Add(wordFrequency.Key, cache);
			}
			LogIndex();
			Log("UpdateExistingIndex ended");
			return result;
		}

		public Dictionary<string, int> GenerateWordFreq(string text)
		{
			return _tokensParser.Parse(text);
		}

		private void LogIndex()
		{
			if(!_detailedTrace)
			{
				return;
			}
			var loadedTrace = _index.Count(x => x.Value.isLoaded);
			var dirtyTrace = _index.Count(x => x.Value.isDirty);
			Log(string.Format("Loaded caches : {0}, Dirty caches: {1}", loadedTrace, dirtyTrace));
		}

		private void Log(string message)
		{
			if (!_detailedTrace)
			{
				return;
			}
			_log(string.Format("[{0}]{1}", _fileName, message));
		}

		#endregion
	}
}
