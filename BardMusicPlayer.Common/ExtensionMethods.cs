/*
 * MogLib/Common/ExtensionMethods.cs
 *
 * Copyright (C) 2021  MoogleTroupe
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BardMusicPlayer.Common
{
    public static class ExtensionMethods
    {
        public static void Append<TK, TV>(this Dictionary<TK, TV> first, Dictionary<TK, TV> second)
        {
            foreach (var item in second)
            {
                first[item.Key] = item.Value;
            }
        }
        public static bool IsSignedNumeral(this object obj, out long value)
        {
            value = 0;
            if (obj == null) return false;
            var type = obj.GetType();
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.SByte:
                    value = (sbyte) obj;
                    return true;
                case TypeCode.Int16:
                    value = (short) obj;
                    return true;
                case TypeCode.Int32:
                    value = (int) obj;
                    return true;
                case TypeCode.Int64:
                    value = (long)obj;
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsUnsignedNumeral(this object obj, out ulong value)
        {
            value = 0;
            if (obj == null) return false;
            var type = obj.GetType();
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Byte:
                    value = (byte) obj;
                    return true;
                case TypeCode.UInt16:
                    value = (ushort) obj;
                    return true;
                case TypeCode.UInt32:
                    value = (uint) obj;
                    return true;
                case TypeCode.UInt64:
                    value = (ulong) obj;
                    return true;
                default:
                    return false;
            }
        }
        public static int Clear<T>(this BlockingCollection<T> blockingCollection)
        {
            if (blockingCollection == null) throw new ArgumentNullException("BlockingCollection");
            var count = 0;
            T _; while (blockingCollection.TryTake(out _)){ count++; }
            return count;
        }
        public static List<string> Split(this string myString, char separator, char escapeCharacter = '\\')
        {
            if (myString.Count(c => c == escapeCharacter) % 2 != 0) myString = myString.Remove(myString.LastIndexOf("" + escapeCharacter, StringComparison.Ordinal), 1);
            return myString.Split(escapeCharacter).Select((element, index) => index % 2 == 0 ? element.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries) : new[] { element }) .SelectMany(element => element).ToList();
        }
        public static MemoryStream Rewind(this MemoryStream memoryStream, long position = 0)
        {
            if (memoryStream == null) throw new EndOfStreamException();
            memoryStream.Position = position;
            return memoryStream;
        }
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));
            var d = new ConcurrentDictionary<TKey, TElement>(comparer ?? EqualityComparer<TKey>.Default);
            foreach (var element in source) d.TryAdd(keySelector(element), elementSelector(element));
            return d;
        }
        public static ConcurrentDictionary<TKey, TSource> ToConcurrentDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => ToConcurrentDictionary<TSource, TKey, TSource>(source, keySelector, IdentityFunction<TSource>.Instance, null);
        public static ConcurrentDictionary<TKey, TSource> ToConcurrentDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) => ToConcurrentDictionary<TSource, TKey, TSource>(source, keySelector, IdentityFunction<TSource>.Instance, comparer);
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) => ToConcurrentDictionary<TSource, TKey, TElement>(source, keySelector, elementSelector, null);
        internal class IdentityFunction<TElement> {
            public static Func<TElement, TElement> Instance
            {
                get { return x => x; }
            }
        }
        public static T Remove<T>(this Stack<T> stack, T element)
        {
            var obj = stack.Pop();
            if (obj.Equals(element))
            {
                return obj;
            }
            var toReturn = stack.Remove(element);
            stack.Push(obj);
            return toReturn;
        }
        public static string FromHex(this string source)
        {
            var builder = new StringBuilder();
            for (var i = 0; i <= source.Length - 2; i += 2)
            {
                builder.Append(Convert.ToChar(int.Parse(source.Substring(i, 2), NumberStyles.HexNumber)));
            }

            return builder.ToString();
        }
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> {
            if(val == null)
                throw new ArgumentNullException(nameof(val), "is null.");
            if(min == null)
                throw new ArgumentNullException(nameof(min), "is null.");
            if(max == null)
                throw new ArgumentNullException(nameof(max), "is null.");
            //If min <= max, clamp
            if(min.CompareTo(max) <= 0)
                return val.CompareTo(min) < 0 ? min : val.CompareTo(max) > 0 ? max : val;
            //If min > max, clamp on swapped min and max
            return val.CompareTo(max) < 0 ? max : val.CompareTo(min) > 0 ? min : val;
        }
    }
}
