using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIDialogBox
    /// </summary>
    public sealed class UIDialogBox : GameObject
    {
        private readonly TextSprite message;
        private readonly DynamicWindow window;

        // Constructor
        public UIDialogBox()
        {
            // Window
            this.window = new()
            {
                FillColor = Color.RosyBrown
            };

            // MessageText
            this.message = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Center,
                MaximumWidth = 140,
                Multiline = true,
                Scale = ScaleInfo.Text.Large
            };
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            window.Height = (int)message.BoundingBox.Height + 8;
            window.Width = (int)message.BoundingBox.Width + 12;
            message.Position = window.InnerBounds.Center;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            window.Draw(gameTime);
            message.Draw(gameTime);
        }

        #endregion

        // Text
        public string? Text
        {
            get => this.message.Text;
            set
            {
                this.message.Text = value;
                Refresh();
            }
        }

        // TextColor
        public Color TextColor
        {
            get => message.Color;
            set => message.Color = value;
        }
    }
}
