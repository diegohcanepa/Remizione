using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle.UI
{
    /// <summary>
    /// UISentence
    /// </summary>
    public sealed class UISentence : GameObject
    {
        private readonly Sprite heartIcon = new(Atlases.UI.HeartIcon) { PivotOrigin = RectanglePoint.Top, Scale = ScaleInfo.UIElement.Medium };
        private readonly TextSprite hpText;
        private readonly TextSprite text;

        // Constructor
        public UISentence()
        {
            text = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -9),
                Scale = ScaleInfo.Text.Huge
            };

            hpText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.Large
            };
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            text.Draw(gameTime);

            if (!hpText.IsEmpty)
            {
                heartIcon.Draw(gameTime);
                hpText.Draw(gameTime);
            }

            Game.SpriteBatch.End();
        }

        // Target
        public GameThing? Target
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    text.Text = field?.DisplayName;
                    hpText.Text = field is Actor actor && actor.MaxHP > 0 ? $"{field.HP}" : null;
                    heartIcon.Position = text.BoundingBox.GetPoint(RectanglePoint.Bottom, -heartIcon.BoundingBox.Width / 2, 0);
                    hpText.Position = heartIcon.BoundingBox.GetPoint(RectanglePoint.Right);
                }
            }
        }
    }
}
