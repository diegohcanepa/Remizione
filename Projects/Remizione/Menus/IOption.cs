using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// IOption
    /// </summary>
    public interface IOption
    {
        string Description { get; }
        void Draw(GameTime gameTime);
        bool IsEnabled { get; }
        RectangleF LabelBoundingBox { get; }
        RectangleF LeftArrowBoundingBox { get; }
        bool NextValue();
        Vector2 Position { get; set; }
        void PerformClick();
        bool PreviousValue();
        RectangleF RightArrowBoundingBox { get; }
    }
}
