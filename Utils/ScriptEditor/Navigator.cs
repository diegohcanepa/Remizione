using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// Navigator
    /// </summary>
    public sealed class Navigator<T> where T : class
    {
        private readonly List<T> list = [];
        private readonly Action onChange;

        // Constructor
        public Navigator(Action onChange)
        {
            this.onChange = onChange;
        }

        // Add
        public bool Add(T item)
        {
            if (item != CurrentItem)
            {
                for (var i = list.Count - 1; i > CurrentIndex; i--)
                {
                    list.RemoveAt(i);
                }

                list.Add(item);
                CurrentIndex++;
                onChange.Invoke();

                return true;
            }
            else
            {
                return false;
            }
        }

        // Back
        public bool Back()
        {
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
                onChange.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        // Clear
        public void Clear()
        {
            list.Clear();
            CurrentIndex = -1;
            onChange.Invoke();
        }

        // Count
        public int Count => list.Count;

        // CurrentIndex
        public int CurrentIndex { get; private set; } = -1;

        // CurrentItem
        public T? CurrentItem => CurrentIndex == -1 ? null : list[CurrentIndex];

        // Forward
        public bool Forward()
        {
            if (CurrentIndex < list.Count - 1)
            {
                CurrentIndex++;
                onChange.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        // RemoveItem
        public void RemoveItem(T item)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == item)
                {
                    list.RemoveAt(i);
                    if (i <= CurrentIndex)
                    {
                        CurrentIndex--;
                    }
                }
            }

            if (list.Count == 0)
            {
                CurrentIndex = -1;
            }
            else
            {
                // Clean up duplicates
                for (var i = list.Count - 1; i > 0; i--)
                {
                    if (list[i - 1] == list[i])
                    {
                        list.RemoveAt(i);
                        if (CurrentIndex >= i)
                        {
                            CurrentIndex--;
                        }
                    }
                }
            }
        }
    }
}
