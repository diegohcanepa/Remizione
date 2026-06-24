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
        private readonly TextSprite text;

        // Constructor
        public UISentence()
        {
            text = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -5),
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            text.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (Target != null)
            {
                if (text.Text != Target.DisplaySentence)
                    text.Text = Target.DisplaySentence;
            }
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
                    text.Text = field?.DisplaySentence;
                }
            }
        }
    }
}
