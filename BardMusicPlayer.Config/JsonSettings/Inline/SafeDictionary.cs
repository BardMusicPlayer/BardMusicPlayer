/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Diagnostics;

namespace BardMusicPlayer.Config.JsonSettings.Inline
{
    /// <summary>
    ///     A dictionary that returns default(T) incase of not existing value.
    ///     And Add will add or set value.
    /// </summary>
    [DebuggerStepThrough]
    internal class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        /// <summary>
        ///     Returns either the value or if not found - the default.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new TValue this[TKey key]
        {
            get
            {
                if (ContainsKey(key))
                    return base[key];
                return default(TValue);
            }
            set { base[key] = value; }
        }


        /// <summary>
        ///     Adds or sets the value to given key.
        /// </summary>
        public new void Add(TKey key, TValue value)
        {
            base[key] = value;
        }

        /// <summary>
        ///     Gets a value via iterating each.<br></br>If not found ,return default(TKey)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TKey FindKeyByValue(TValue value)
        {
            foreach (var pair in this)
                if (value.Equals(pair.Value)) return pair.Key;

            return default(TKey);
        }

        public SafeDictionary<TKey, TValue> Clone()
        {
            return new SafeDictionary<TKey, TValue>(this);
        }

        public new IEnumerable<TValue> Values()
        {
            return base.Values;
        }

        public new IEnumerable<TKey> Keys()
        {
            return base.Keys;
        }

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that is
        ///     empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public SafeDictionary() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that is
        ///     empty, has the specified initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="capacity">
        ///     The initial number of elements that the <see cref="T:System.Collections.Generic.Dictionary`2" />
        ///     can contain.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="capacity" /> is less than 0.
        /// </exception>
        public SafeDictionary(int capacity) : base(capacity) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that is
        ///     empty, has the default initial capacity, and uses the specified
        ///     <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.
        /// </summary>
        /// <param name="comparer">
        ///     The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when
        ///     comparing keys, or null to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the
        ///     type of the key.
        /// </param>
        public SafeDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that is
        ///     empty, has the specified initial capacity, and uses the specified
        ///     <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.
        /// </summary>
        /// <param name="capacity">
        ///     The initial number of elements that the <see cref="T:System.Collections.Generic.Dictionary`2" />
        ///     can contain.
        /// </param>
        /// <param name="comparer">
        ///     The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when
        ///     comparing keys, or null to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the
        ///     type of the key.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="capacity" /> is less than 0.
        /// </exception>
        public SafeDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that contains
        ///     elements copied from the specified <see cref="T:System.Collections.Generic.IDictionary`2" /> and uses the default
        ///     equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">
        ///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the
        ///     new <see cref="T:System.Collections.Generic.Dictionary`2" />.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="dictionary" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="dictionary" /> contains one or more duplicate keys.
        /// </exception>
        public SafeDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that contains
        ///     elements copied from the specified <see cref="T:System.Collections.Generic.IDictionary`2" /> and uses the specified
        ///     <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.
        /// </summary>
        /// <param name="dictionary">
        ///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the
        ///     new <see cref="T:System.Collections.Generic.Dictionary`2" />.
        /// </param>
        /// <param name="comparer">
        ///     The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when
        ///     comparing keys, or null to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> for the
        ///     type of the key.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="dictionary" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="dictionary" /> contains one or more duplicate keys.
        /// </exception>
        public SafeDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

        #endregion
    }
}
