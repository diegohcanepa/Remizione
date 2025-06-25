using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// ITransform
    /// </summary>
    public interface ITransform : IBoundingBox
    {
        int Height { get; }
        bool IsFlippedHorizontally { get; }
        bool IsFlippedVertically { get; }
        RectanglePoint PivotOrigin { get; set; }
        Vector2 Position { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Rotation { get; set; }
        Vector2 Scale { get; set; }
        float ScaleX { get; set; }
        float ScaleY { get; set; }
        int Width { get; }
    }
}
