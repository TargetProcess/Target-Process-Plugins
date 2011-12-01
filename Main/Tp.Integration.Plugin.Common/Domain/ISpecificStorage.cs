// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tp.Core;

namespace Tp.Integration.Plugin.Common.Domain
{
	/// <summary>
	/// Provides access to stored objects of provided type as a collection of items.
	/// </summary>
	/// <typeparam name="T">The type of stored objects.</typeparam>
	public interface IStorage<T> : INullable, IEnumerable<T>
	{
		/// <summary>
		/// Replace all stored objects with provided array.
		/// </summary>
		/// <param name="value">The new array of objects to be stored.</param>
		void ReplaceWith(params T[] value);

		void Update(T value, Predicate<T> condition);

		/// <summary>
		/// Adds range of items to the storage.
		/// </summary>
		/// <param name="items">Items to add to the storage.</param>
		void AddRange(IEnumerable<T> items);

		void Remove(Predicate<T> condition);

		void Add(T item);

		void Clear();
	}

	public class StorageSafeNull<T> : SafeNull<StorageSafeNull<T>, IStorage<T>>, IStorage<T>
	{
		public void ReplaceWith(params T[] value)
		{
		}

		public void Update(T value, Predicate<T> condition)
		{
		}

		public void AddRange(IEnumerable<T> items)
		{
		}

		public void Remove(Predicate<T> condition)
		{
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new T[] {}.AsEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Expression Expression
		{
			get { return new ExpressionSafeNull<T>(); }
		}

		public class ExpressionSafeNull<T1> : Expression
		{
			public ExpressionSafeNull() : base(ExpressionType.Call, typeof (T1))
			{
			}
		}

		public Type ElementType
		{
			get { return typeof (T); }
		}

		public IQueryProvider Provider
		{
			get { return new T[] {}.AsQueryable().Provider; }
		}

		public void Add(T item)
		{
		}

		public void Clear()
		{
		}

		public bool Contains(T item)
		{
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
		}

		public bool Remove(T item)
		{
			return false;
		}

		public int Count
		{
			get { return 0; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}
	}
}