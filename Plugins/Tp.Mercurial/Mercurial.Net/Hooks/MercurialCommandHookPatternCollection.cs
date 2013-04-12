using System;
using System.Collections;
using System.Collections.Generic;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This class is used by <see cref="MercurialPreCommandHook"/> and <see cref="MercurialPostCommandHook"/> to
    /// hold the options to the Mercurial command.
    /// </summary>
    public sealed class MercurialCommandHookPatternCollection : MercurialCommandHookDataStructureBase, IList<object>
    {
        /// <summary>
        /// This is the backing field for this <see cref="MercurialCommandHookPatternCollection"/>.
        /// </summary>
        private readonly List<object> _Collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialCommandHookPatternCollection"/> class.
        /// </summary>
        /// <param name="listValue">
        /// The current value of the list, as a string.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>The <paramref name="listValue"/> is <c>null</c> or an empty string.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>The <paramref name="listValue"/> is <c>null</c> or an empty string.</para>
        /// <para>- or -</para>
        /// <para>The <paramref name="listValue"/> must start with an opening curly brace.</para>
        /// <para>- or -</para>
        /// <para>The <paramref name="listValue"/> must end with a closing curly brace.</para>
        /// <para>- or -</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>Unsupported syntax in the <paramref name="listValue"/>.</para>
        /// </exception>
        public MercurialCommandHookPatternCollection(string listValue)
        {
            if (listValue == null)
                throw new ArgumentNullException("listValue");
            if (!listValue.StartsWith("[", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("listValue must start with an opening curly brace", "listValue");
            if (!listValue.EndsWith("]", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("listValue must end with a closing curly brace", "listValue");

            int index = 0;
            _Collection = new List<object>((object[])ParseValue(listValue, ref index));
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">
        /// The object to locate in the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </param>
        public int IndexOf(object item)
        {
            return _Collection.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="MercurialCommandHookPatternCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which <paramref name="item"/> should be inserted.
        /// </param>
        /// <param name="item">
        /// The object to insert into the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="MercurialCommandHookPatternCollection"/> is read-only.
        /// </exception>
        public void Insert(int index, object item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the <see cref="MercurialCommandHookPatternCollection"/> item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="MercurialCommandHookPatternCollection"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="MercurialCommandHookPatternCollection"/> is read-only.
        /// </exception>
        public object this[int index]
        {
            get
            {
                return _Collection[index];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="MercurialCommandHookPatternCollection"/> is read-only.
        /// </exception>
        public void Add(object item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="MercurialCommandHookPatternCollection"/> is read-only. 
        /// </exception>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the <see cref="MercurialCommandHookPatternCollection"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="MercurialCommandHookPatternCollection"/>; otherwise, false.
        /// </returns>
        /// <param name="item">
        /// The object to locate in the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </param>
        public bool Contains(object item)
        {
            return _Collection.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="MercurialCommandHookPatternCollection"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="MercurialCommandHookPatternCollection"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="array"/> is multidimensional.</para>
        /// <para>- or -</para>
        /// <para><paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.</para>
        /// <para>- or -</para>
        /// <para>The number of elements in the source <see cref="MercurialCommandHookPatternCollection"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</para>
        /// </exception>
        public void CopyTo(object[] array, int arrayIndex)
        {
            _Collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return _Collection.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MercurialCommandHookPatternCollection"/> is read-only.
        /// </summary>
        /// <returns>
        /// This property always returns <c>true</c> for <see cref="MercurialCommandHookPatternCollection"/>.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="MercurialCommandHookPatternCollection"/>;
        /// otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="MercurialCommandHookPatternCollection"/>.
        /// </returns>
        /// <param name="item">
        /// The object to remove from the <see cref="MercurialCommandHookPatternCollection"/>.
        /// </param>
        /// <exception cref="NotSupportedException">The <see cref="MercurialCommandHookPatternCollection"/> is read-only.
        /// </exception>
        public bool Remove(object item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<object> GetEnumerator()
        {
            return _Collection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
