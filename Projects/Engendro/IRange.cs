using System;

namespace Engendro
{
    /// <summary>
    /// IRange
    /// </summary>
    public interface IRange<T> where T : IComparable, IComparable<T>, IEquatable<T>
    {
        bool Contains(T value);
        bool IsEmpty { get; }
        T Delta { get; }
        T Maximum { get; }
        T Minimum { get; }
        T Random();
    }
}