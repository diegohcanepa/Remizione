using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// PathSegment
    /// </summary>
    public sealed class PathSegment
    {
        // Constructor
        public PathSegment()
        {
        }

        // Constructor
        public PathSegment(Vector2 start, Vector2 end)
        {
            this.Start = start;
            this.End = end;
        }

        // End
        public Vector2 End { get; private set; }

        // IsEmpty
        public bool IsEmpty => Start == Vector2.Zero && End == Vector2.Zero;

        // Reset
        public void Reset()
        {
            Start = Vector2.Zero;
            End = Vector2.Zero;
        }

        // SetPath
        public void SetPath(Vector2 start, Vector2 end)
        {
            this.Start = start;
            this.End = end;
        }

        // Start
        public Vector2 Start { get; private set; }

        // ToString
        public override string ToString()
        {
            return $"{Start} -> {End}";
        }
    }
}
