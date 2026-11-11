using System;

namespace Engendro.PathFinding
{
    /// <summary>
    /// IHeapItem
    /// </summary>
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}
