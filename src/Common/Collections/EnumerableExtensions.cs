﻿/*
 * Copyright 2006-2014 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NanoByte.Common.Properties;
using NanoByte.Common.Values;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Provides extension methods for <see cref="IEnumerable{T}"/>s.
    /// </summary>
    public static class EnumerableExtensions
    {
        #region LINQ
        /// <summary>
        /// Filters a sequence of elements to remove any that match the <paramref name="predicate"/>.
        /// The opposite of <see cref="Enumerable.Where{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,bool})"/>.
        /// </summary>
        [NotNull, LinqTunnel]
        public static IEnumerable<T> Except<T>([NotNull] this IEnumerable<T> enumeration, [NotNull] Func<T, bool> predicate)
        {
            return enumeration.Where(x => !predicate(x));
        }

        /// <summary>
        /// Filters a sequence of elements to remove any that are equal to <paramref name="element"/>.
        /// </summary>
        [NotNull, LinqTunnel]
        public static IEnumerable<T> Except<T>([NotNull] this IEnumerable<T> enumeration, T element)
        {
            return enumeration.Except(new[] {element});
        }

        /// <summary>
        /// Flattens a list of lists.
        /// </summary>
        [NotNull, LinqTunnel]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static IEnumerable<T> Flatten<T>([NotNull] this IEnumerable<IEnumerable<T>> enumeration)
        {
            return enumeration.SelectMany(x => x);
        }

        /// <summary>
        /// Appends an element to a list.
        /// </summary>
        [NotNull, LinqTunnel]
        public static IEnumerable<T> Append<T>([NotNull] this IEnumerable<T> enumeration, T element)
        {
            return enumeration.Concat(new[] {element});
        }

        /// <summary>
        /// Prepends an element to a list.
        /// </summary>
        [NotNull, LinqTunnel]
        public static IEnumerable<T> Prepend<T>([NotNull] this IEnumerable<T> enumeration, T element)
        {
            return new[] {element}.Concat(enumeration);
        }

        /// <summary>
        /// Filters a sequence of elements to remove any <see langword="null"/> values.
        /// </summary>
        [NotNull, ItemNotNull, LinqTunnel]
        public static IEnumerable<T> WhereNotNull<T>([NotNull] this IEnumerable<T> enumeration)
        {
            return enumeration.Where(element => element != null);
        }

        /// <summary>
        /// Filters a sequence of elements to remove any duplicates based on the equality of a key extracted from the elements.
        /// </summary>
        /// <param name="enumeration">The sequence of elements to filter.</param>
        /// <param name="keySelector">A function mapping elements to their respective equality keys.</param>
        [NotNull, LinqTunnel]
        public static IEnumerable<T> Distinct<T, TKey>([NotNull] this IEnumerable<T> enumeration, [NotNull] Func<T, TKey> keySelector)
        {
            return enumeration.Distinct(new KeyEqualityComparer<T, TKey>(keySelector));
        }

        /// <summary>
        /// Maps elements like <see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TResult})"/>, but with exception handling.
        /// </summary>
        /// <typeparam name="TSource">The type of the input elements.</typeparam>
        /// <typeparam name="TResult">The type of the output elements.</typeparam>
        /// <typeparam name="TException">The type of exceptions to ignore. Any other exceptions are passed through.</typeparam>
        /// <param name="source">The elements to map.</param>
        /// <param name="selector">The selector to execute for each <paramref name="source"/> element. When it throws <typeparamref name="TException"/> the element is skipped. Any other exceptions are passed through.</param>
        [NotNull, LinqTunnel]
        public static IEnumerable<TResult> TrySelect<TSource, TResult, TException>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, TResult> selector)
            where TException : Exception
        {
            #region Sanity checks
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            #endregion

            foreach (var element in source)
            {
                TResult result;
                try
                {
                    result = selector(element);
                }
                catch (TException)
                {
                    continue;
                }
                yield return result;
            }
        }
        #endregion

        #region Equality
        /// <summary>
        /// Determines whether two collections contain the same elements in the same order.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        public static bool SequencedEquals<T>([NotNull, InstantHandle] this ICollection<T> first, [NotNull, InstantHandle] ICollection<T> second, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            #endregion

            if (first.Count != second.Count) return false;
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            return first.SequenceEqual(second, comparer);
        }

        /// <summary>
        /// Determines whether two collections contain the same elements disregarding the order they are in.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        public static bool UnsequencedEquals<T>([NotNull, InstantHandle] this ICollection<T> first, [NotNull, InstantHandle] ICollection<T> second, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            #endregion

            if (first.Count != second.Count) return false;
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            if (first.GetUnsequencedHashCode(comparer) != second.GetUnsequencedHashCode(comparer)) return false;
            return first.All(x => second.Contains(x, comparer));
        }

        /// <summary>
        /// Generates a hash code for the contents of a collection. Changing the elements' order will change the hash.
        /// </summary>
        /// <param name="collection">The collection to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        /// <seealso cref="SequencedEquals{T}(System.Collections.Generic.ICollection{T},System.Collections.Generic.ICollection{T},System.Collections.Generic.IEqualityComparer{T})"/>
        public static int GetSequencedHashCode<T>([NotNull, InstantHandle] this IEnumerable<T> collection, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException("collection");
            #endregion

            if (comparer == null) comparer = EqualityComparer<T>.Default;

            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (T unknown in collection.WhereNotNull())
                    result = (result * 397) ^ comparer.GetHashCode(unknown);
                return result;
            }
        }

        /// <summary>
        /// Generates a hash code for the contents of a collection. Changing the elements' order will not change the hash.
        /// </summary>
        /// <param name="collection">The collection to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        /// <seealso cref="UnsequencedEquals{T}"/>
        public static int GetUnsequencedHashCode<T>([NotNull, InstantHandle] this IEnumerable<T> collection, [CanBeNull, InstantHandle] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException("collection");
            #endregion

            if (comparer == null) comparer = EqualityComparer<T>.Default;

            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (T unknown in collection.WhereNotNull())
                    result = result ^ comparer.GetHashCode(unknown);
                return result;
            }
        }
        #endregion

        #region Clone
        /// <summary>
        /// Calls <see cref="ICloneable.Clone"/> for every element in a collection and returns the results as a new collection.
        /// </summary>
        public static IEnumerable<T> CloneElements<T>([NotNull, ItemNotNull] this IEnumerable<T> enumerable) where T : ICloneable
        {
            return enumerable.Select(entry => (T)entry.Clone());
        }
        #endregion

        #region Transactions
        /// <summary>
        /// Applies an operation for all elements of a collection. Automatically applies rollback operations in case of an exception.
        /// </summary>
        /// <typeparam name="T">The type of elements to operate on.</typeparam>
        /// <param name="elements">The elements to apply the action for.</param>
        /// <param name="apply">The action to apply to each element.</param>
        /// <param name="rollback">The action to apply to each element that <paramref name="apply"/> was called on in case of an exception.</param>
        /// <remarks>
        /// <paramref name="rollback"/> is applied to the element that raised an exception in <paramref name="apply"/> and then interating backwards through all previous elements.
        /// After rollback is complete the exception is passed on.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Suppress exceptions during rollback since they would hide the actual exception that caused the rollback in the first place")]
        public static void ApplyWithRollback<T>([NotNull, InstantHandle] this IEnumerable<T> elements, [NotNull, InstantHandle] Action<T> apply, [NotNull, InstantHandle] Action<T> rollback)
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException("elements");
            if (apply == null) throw new ArgumentNullException("apply");
            if (rollback == null) throw new ArgumentNullException("rollback");
            #endregion

            var rollbackJournal = new LinkedList<T>();
            try
            {
                foreach (var element in elements)
                {
                    // Remember the element for potential rollback
                    rollbackJournal.AddFirst(element);

                    apply(element);
                }
            }
            catch
            {
                foreach (var element in rollbackJournal)
                {
                    try
                    {
                        rollback(element);
                    }
                    catch (Exception ex)
                    {
                        // Suppress exceptions during rollback since they would hide the actual exception that caused the rollback in the first place
                        Log.Error(string.Format(Resources.FailedToRollback, element));
                        Log.Error(ex);
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// Applies an operation for the first possible element of a collection.
        /// If the operation succeeds the remaining elements are ignored. If the operation fails it is repeated for the next element.
        /// </summary>
        /// <typeparam name="T">The type of elements to operate on.</typeparam>
        /// <param name="elements">The elements to apply the action for.</param>
        /// <param name="action">The action to apply to an element.</param>
        /// <exception cref="Exception">The exception thrown by <paramref name="action"/> for the last element of <paramref name="elements"/>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Last excption is rethrown, other exceptions are logged")]
        public static void Try<T>([NotNull, InstantHandle] this IEnumerable<T> elements, [NotNull, InstantHandle] Action<T> action)
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException("elements");
            if (action == null) throw new ArgumentNullException("action");
            #endregion

            var enumerator = elements.GetEnumerator();
            if (!enumerator.MoveNext()) return;

            while (true)
            {
                try
                {
                    action(enumerator.Current);
                    return;
                }
                catch (Exception ex)
                {
                    if (enumerator.MoveNext()) Log.Error(ex); // Log exception and try next element
                    else throw; // Rethrow exception if there are no more elements
                }
            }
        }
        #endregion
    }
}
