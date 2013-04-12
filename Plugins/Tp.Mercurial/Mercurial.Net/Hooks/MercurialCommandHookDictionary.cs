using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This class is used by <see cref="MercurialPreCommandHook"/> and <see cref="MercurialPostCommandHook"/> to
    /// hold the options to the Mercurial command.
    /// </summary>
    public sealed class MercurialCommandHookDictionary : MercurialCommandHookDataStructureBase, IDictionary<string, object>
    {
        /// <summary>
        /// This is the backing field for this <see cref="MercurialCommandHookDictionary"/>.
        /// </summary>
        private readonly Dictionary<string, object> _Collection = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialCommandHookDictionary"/> class.
        /// </summary>
        /// <param name="dictionaryValue">
        /// The current value of the dictionary, as a string.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>The <paramref name="dictionaryValue"/> is <c>null</c> or an empty string.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>The <paramref name="dictionaryValue"/> is <c>null</c> or an empty string.</para>
        /// <para>- or -</para>
        /// <para>The <paramref name="dictionaryValue"/> must start with an opening curly brace.</para>
        /// <para>- or -</para>
        /// <para>The <paramref name="dictionaryValue"/> must end with a closing curly brace.</para>
        /// <para>- or -</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>Unsupported syntax in the <paramref name="dictionaryValue"/>.</para>
        /// </exception>
        public MercurialCommandHookDictionary(string dictionaryValue)
        {
            if (dictionaryValue == null)
                throw new ArgumentNullException("dictionaryValue");
            if (!dictionaryValue.StartsWith("{", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("dictionaryValue must start with an opening curly brace", "dictionaryValue");
            if (!dictionaryValue.EndsWith("}", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("dictionaryValue must end with a closing curly brace", "dictionaryValue");

            int index = 1;
            while (index < dictionaryValue.Length && dictionaryValue[index] != '}')
            {
                int prevIndex = index;

                // Grab name of key/value pair
                var key = ParseValue(dictionaryValue, ref index) as string;
                if (prevIndex == index)
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported syntax in options combined at position #{0} (0-based), did not understand value at location", index));
                if (key == null)
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported syntax in options combined at position #{0} (0-based), expected name as string", prevIndex));

                // Skip colon
                if (dictionaryValue[index] != ':')
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported syntax in options combined at position #{0} (0-based), expected colon (:)", prevIndex));
                index++;

                // Skip space
                if (dictionaryValue[index] != ' ')
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported syntax in options combined at position #{0} (0-based), expected space after colon", prevIndex));
                index++;

                // Grab value
                prevIndex = index;
                object value = ParseValue(dictionaryValue, ref index);
                if (prevIndex == index)
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported syntax in options combined at position #{0} (0-based), did not understand value at location", index));

                // Store
                _Collection[key] = value;

                // Skip comma and space, if present
                if (dictionaryValue[index] == ',')
                {
                    index++;
                    if (dictionaryValue[index] == ' ')
                        index++;
                }
            }
        }

        /// <summary>
        /// Always throws <see cref="NotSupportedException"/> since this <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </summary>
        /// <param name="key">
        /// The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        /// The object to use as the value of the element to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see cref="MercurialCommandHookDictionary"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </exception>
        public void Add(string key, object value)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>
        /// Determines whether the <see cref="MercurialCommandHookDictionary"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="MercurialCommandHookDictionary"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">
        /// The key to locate in the <see cref="MercurialCommandHookDictionary"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public bool ContainsKey(string key)
        {
            return _Collection.ContainsKey(key);
        }

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the keys of the <see cref="MercurialCommandHookDictionary"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection{T}"/> containing the keys of this <see cref="MercurialCommandHookDictionary"/>.
        /// </returns>
        public ICollection<string> Keys
        {
            get
            {
                return _Collection.Keys;
            }
        }

        /// <summary>
        /// Always throws <see cref="NotSupportedException"/> since this <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="MercurialCommandHookDictionary"/>.
        /// </returns>
        /// <param name="key">
        /// The key of the element to remove.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </exception>
        public bool Remove(string key)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if this <see cref="MercurialCommandHookDictionary"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">
        /// The key whose value to get.
        /// </param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public bool TryGetValue(string key, out object value)
        {
            return _Collection.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the values in the <see cref="MercurialCommandHookDictionary"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection{T}"/> containing the values in the object that implements <see cref="MercurialCommandHookDictionary"/>.
        /// </returns>
        public ICollection<object> Values
        {
            get
            {
                return _Collection.Values;
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">
        /// The key of the element to get or set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and <paramref name="key"/> is not found.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </exception>
        public object this[string key]
        {
            get
            {
                return _Collection[key];
            }

            set
            {
                throw new NotSupportedException("This dictionary is read-only");
            }
        }

        /// <summary>
        /// Always throws <see cref="NotSupportedException"/> since this <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="MercurialCommandHookDictionary"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </exception>
        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>
        /// Always throws <see cref="NotSupportedException"/> since this <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="MercurialCommandHookDictionary"/> is read-only. 
        /// </exception>
        public void Clear()
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>
        /// Determines whether the <see cref="MercurialCommandHookDictionary"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="MercurialCommandHookDictionary"/>; otherwise, false.
        /// </returns>
        /// <param name="item">
        /// The object to locate in the <see cref="MercurialCommandHookDictionary"/>.
        /// </param>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return _Collection.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="MercurialCommandHookDictionary"/> to 
        /// an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from
        /// <see cref="MercurialCommandHookDictionary"/>.
        /// The <see cref="Array"/> must have zero-based indexing.
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
        /// <para>The number of elements in the source <see cref="MercurialCommandHookDictionary"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</para>
        /// <para>- or -</para>
        /// <para>Value cannot be cast automatically to the type of the destination <paramref name="array"/>.</para>
        /// </exception>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)_Collection).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="MercurialCommandHookDictionary"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="MercurialCommandHookDictionary"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return _Collection.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </summary>
        /// <returns>
        /// Always true since the <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Always throws <see cref="NotSupportedException"/> since this <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="MercurialCommandHookDictionary"/>; otherwise, false.
        /// This method also returns false if <paramref name="item"/> is not found in the original <see cref="MercurialCommandHookDictionary"/>.
        /// </returns>
        /// <param name="item">
        /// The object to remove from the <see cref="MercurialCommandHookDictionary"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="MercurialCommandHookDictionary"/> is read-only.
        /// </exception>
        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _Collection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
