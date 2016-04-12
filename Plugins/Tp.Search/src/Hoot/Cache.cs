// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace hOOt
{
	public class IndexData
	{
		public int DocNumber { get; set; }
		public IEnumerable<string> Words { get; set; }
	}
	public class IndexResult
	{
		public IndexResult()
		{
			DocNumber = -1;
			WordsAdded = new Dictionary<string, int>();
			WordsRemoved = new List<string>();
		}

		public int DocNumber { get; set; }
		public Dictionary<string, int> WordsAdded { get; set; }
		public List<string> WordsRemoved { get; set; }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class FilterablePropertyAttribute : Attribute
	{
		private readonly string _name;

		public FilterablePropertyAttribute(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}
	}

	public class Document
	{
		public Document()
		{
			DocNumber = -1;
		}

		public Document(string filename, string text)
		{
			FileName = filename;
			Text = text;
			DocNumber = -1;
		}

		public int DocNumber { get; set; }

		[XmlIgnore]
		public string Text { get; set; }

		public string FileName { get; set; }

		public override string ToString()
		{
			return FileName;
		}
	}

	internal class Cache
	{
		public enum OPERATION
		{
			AND,
			OR,
			ANDNOT
		}

		public bool isLoaded = false;
		public bool isDirty = true;
		public long FileOffset = -1;
		public int LastBitSaveLength = 0;
		private WAHBitArray _bits;

		public void SetBit(int index, bool val)
		{
			if (_bits != null)
				_bits.Set(index, val);
			else
			{
				_bits = new WAHBitArray();
				_bits.Set(index, val);
			}
			isDirty = true;
		}

		public uint[] GetCompressedBits()
		{
			return _bits != null ? _bits.GetCompressed() : null;
		}

		public void FreeMemory(bool unload, bool freeUncompressedMemory = true)
		{
			if (freeUncompressedMemory && _bits != null)
				_bits.FreeMemory();
			if (unload)
			{
				_bits = null;
				isLoaded = false;
			}
		}

		public void SetCompressedBits(uint[] bits)
		{
			_bits = new WAHBitArray(WAHBitArray.TYPE.Compressed_WAH, bits);
			LastBitSaveLength = bits.Length;
			isLoaded = true;
			isDirty = false;
		}

		public WAHBitArray Op(WAHBitArray bits, OPERATION op)
		{
			if (bits == null)
			{
				throw new InvalidOperationException("Bits is null");
			}
			if (op == OPERATION.AND)
				return _bits.And(bits);
			if (op == OPERATION.OR)
				return _bits.Or(bits);
			return bits.And(_bits.Not());
		}

		public WAHBitArray GetBitmap()
		{
			return _bits;
		}
	}
}