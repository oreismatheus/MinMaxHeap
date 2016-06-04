﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MinMaxHeap
{
    /// <typeparam name="TDictionary">
    /// Maps a key to the index of the corresponding KeyValuePair 
    /// in the list.</typeparam>
    public class MinHeap<TKey, TValue, TDictionary>
        where TDictionary : IDictionary<TKey, int>, new()
    {
        List<KeyValuePair<TKey, TValue>> values;
        TDictionary indexInList;
        IComparer<TValue> comparer;

        public MinHeap(IEnumerable<KeyValuePair<TKey, TValue>> items,
            IComparer<TValue> comparer)
        {
            values = new List<KeyValuePair<TKey, TValue>>();
            indexInList = new TDictionary();
            this.comparer = comparer;
            values.Add(default(KeyValuePair<TKey, TValue>));
            addItems(items);
        }

        public MinHeap(IEnumerable<KeyValuePair<TKey, TValue>> items)
            : this(items, Comparer<TValue>.Default)
        { }

        public MinHeap(IComparer<TValue> comparer)
            : this(new KeyValuePair<TKey, TValue>[0], comparer)
        { }

        public MinHeap() : this(Comparer<TValue>.Default)
        { }

        public int Count
        {
            get
            {
                return values.Count - 1;
            }
        }

        public KeyValuePair<TKey, TValue> Min
        {
            get
            {
                return values[1];
            }
        }

        /// <summary>
        /// Extract the smallest element.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public KeyValuePair<TKey, TValue> ExtractMin()
        {
            int count = Count;

            if (count == 0)
            {
                throw new InvalidOperationException("Heap is empty.");
            }

            var min = Min;
            values[1] = values[count];
            values.RemoveAt(count);
            indexInList.Remove(min.Key);

            if (values.Count > 1)
            {
                indexInList[values[1].Key] = 1;
                bubbleDown(1);
            }

            return min;
        }

        /// <summary>
        /// Insert the key and value.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(TKey key, TValue val)
        {
            int count = values.Count;
            indexInList.Add(key, count);
            values.Add(new KeyValuePair<TKey, TValue>(key, val));
            bubbleUp(count);
        }

        /// <summary>
        /// Modify the value corresponding to the given key.
        /// </summary>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public void ChangeValue(TKey key, TValue newValue)
        {
            int index = indexInList[key];
            int compareVal = comparer.Compare(newValue, values[index].Value);
            values[index] = new KeyValuePair<TKey, TValue>(
                values[index].Key, newValue);

            if (compareVal > 0)
            {
                bubbleDown(index);
            }
            else if (compareVal < 0)
            {
                bubbleUp(index);
            }
        }

        /// <summary>
        /// Returns whether the key exists.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public bool ContainsKey(TKey key)
        {
            return indexInList.ContainsKey(key);
        }

        /// <summary>
        /// Gets the KeyValuePair correspoinding to the given key.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public KeyValuePair<TKey, TValue> this[TKey key]
        {
            get
            {
                return values[indexInList[key]];
            }
        }

        private void bubbleUp(int index)
        {
            int parent = index / 2;

            while (
                index > 1 &&
                compareResult(parent, index) > 0)
            {
                exchange(index, parent);
                index = parent;
                parent /= 2;
            }
        }

        private void bubbleDown(int index)
        {
            int min;

            while (true)
            {
                int left = index * 2;
                int right = index * 2 + 1;

                if (left < values.Count &&
                    compareResult(left, index) < 0)
                {
                    min = left;
                }
                else
                {
                    min = index;
                }

                if (right < values.Count &&
                    compareResult(right, min) < 0)
                {
                    min = right;
                }

                if (min != index)
                {
                    exchange(index, min);
                    index = min;
                }
                else
                {
                    return;
                }
            }
        }

        // JIT compiler does not inline this method without this 
        // attribute. Inlining gives a small (about 5%) performance
        // increase.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int compareResult(int index1, int index2)
        {
            return comparer.Compare(
                values[index1].Value, values[index2].Value);
        }

        private void exchange(int index, int max)
        {
            var tmp = values[index];
            values[index] = values[max];
            values[max] = tmp;

            indexInList[values[index].Key] = index;
            indexInList[values[max].Key] = max;
        }

        private void addItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            int index = values.Count;

            foreach (var i in items)
            {
                values.Add(i);
                indexInList.Add(i.Key, index);
                index++;
            }

            for (int i = values.Count / 2; i >= 1; i--)
            {
                bubbleDown(i);
            }
        }
    }
}
