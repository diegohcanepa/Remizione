namespace Engendro.PathFinding
{
    /// <summary>
    /// Heap
    /// </summary>
    public sealed class Heap<T>(int maxHeapSize) where T : IHeapItem<T>
    {
        private readonly T[] items = new T[maxHeapSize];
        private int currentItemCount;

        #region Private members

        // SortDown
        private void SortDown(T item)
        {
            while (true)
            {
                var childIndexLeft = item.HeapIndex * 2 + 1;
                var childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex;

                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        // SortUp
        private void SortUp(T item)
        {
            var parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                var parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        // Swap
        private void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            var itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }

        #endregion

        // Add
        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }

        // Contains
        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        // Count
        public int Count => currentItemCount;

        // RemoveFirst
        public T RemoveFirst()
        {
            var firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }

        // Update
        public void UpdateItem(T item)
        {
            SortUp(item);
        }
    }
}