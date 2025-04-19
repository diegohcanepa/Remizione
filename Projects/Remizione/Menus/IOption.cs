using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.Menus
{
    /// <summary>
    /// IOption
    /// </summary>
    public interface IOption : IDraw, IUpdate
    {
        string Description { get; }
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
