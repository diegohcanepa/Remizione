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

        // Available
        public T[] Available => pool.ToArray();

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
            var index = inUse.IndexOf(obj);
            if (index == -1)
                return;

            inUse.RemoveAt(index);

            if (pool.Count < maxSize)
                pool.Enqueue(obj);
        }
    }
}
