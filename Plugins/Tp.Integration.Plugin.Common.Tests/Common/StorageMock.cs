// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	public class StorageMock<T> : IStorage<T>
	{
		private readonly List<object> _value;

		public StorageMock(List<object> storage)
		{
			_value = storage;
		}

		public void ReplaceWith(params object[] value)
		{
			Clear();
			_value.AddRange(value);
		}

		public void ReplaceWith(params T[] value)
		{
			_value.RemoveAll(x => x.GetType() == typeof (T));
			_value.AddRange(value.ToList().ConvertAll(x => (object) x));
		}

		public void Save()
		{
		}

		public void Update(T value)
		{
			_value[_value.IndexOf(_value.First(x => x.Equals(value)))] = value;
		}

		public void Update(T value, Predicate<T> condition)
		{
			_value[_value.IndexOf(_value.First(x => x.GetType() == typeof(T) && condition((T)x)))] = value;
		}

		public void AddRange(IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}

		public void Remove(Predicate<T> condition)
		{
			_value.RemoveAll(x => x.GetType() == typeof (T) && condition((T)x));
		}

		public bool IsNull
		{
			get { return false; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _value.OfType<T>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Expression Expression
		{
			get { return _value.OfType<T>().AsQueryable().Expression; }
		}

		public Type ElementType
		{
			get { return typeof (T); }
		}

		public IQueryProvider Provider
		{
			get { return _value.OfType<T>().AsQueryable().Provider; }
		}

		public void Add(T item)
		{
			_value.Add(item);
		}

		public void Clear()
		{
			_value.RemoveAll(x => x.GetType() == typeof (T));
		}

		public bool Contains(T item)
		{
			return _value.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			var values2Copy = _value.OfType<T>().Skip(arrayIndex).ToArray();
			values2Copy.CopyTo(array, 0);
		}

		public bool Remove(T item)
		{
			return _value.Remove(item);
		}

		public int Count
		{
			get { return _value.OfType<T>().Count(); }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}
	}
}