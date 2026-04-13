using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// ObjectPool
    /// </summary>
    public class ObjectPool<T> where T : class
    {
        private readonly List<T> inUse = [];
        private readonly int maxSize;
        private readonly Func<T> objectGenerator;
        private readonly Queue<T> pool = new();

        // Constructor
        public ObjectPool(Func<T> objectGenerator, int maxSize, int precachedSize = 0)
        {
            CodeContract.GreaterThanZero(maxSize, nameof(maxSize));

            this.maxSize = maxSize;
            this.objectGenerator = objectGenerator;
            this.InUse = new ReadOnlyCollection<T>(inUse);

            for (var i = 0; i < precachedSize; i++)
            {
                pool.Enqueue(objectGenerator());
            }
        }

        // AvailableCount
        public int AvailableCount => pool.Count;

        // Get
        public T Get()
        {
            T obj = pool.Count > 0 ? pool.Dequeue() : objectGenerator();
            inUse.Add(obj);
            return obj;
        }

        // InUse
        public ReadOnlyCollection<T> InUse { get; }

        // Return
        public void Return(T obj)
        {
            if (!inUse.Remove(obj))
                return;

            // Optional reset
            if (obj is IPoolable poolable)
                poolable.Reset();

            if (pool.Count < maxSize)
                pool.Enqueue(obj);
        }

        // Return
        public void Return(IList<T> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                Return(items[i]);
            }
        }

        // ReturnAll
        public void ReturnAll()
        {
            int count = inUse.Count;

            for (int i = 0; i < count; i++)
            {
                T obj = inUse[i];

                if (obj is IPoolable poolable)
                    poolable.Reset();

                if (pool.Count < maxSize)
                    pool.Enqueue(obj);
            }

            inUse.Clear();
        }
    }
}
