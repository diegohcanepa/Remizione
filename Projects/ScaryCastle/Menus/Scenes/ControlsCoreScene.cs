using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// ControlsCoreScene
    /// </summary>
    public abstract class ControlsCoreScene : StandardMenuScene
    {
        private readonly ImageSprite image;
        private readonly TextSprite[] labels;
        protected enum LabelName { Interact, Inventory, Movement, MovementAlt, Run, Kick, Menu }

        #region Constructor

        // Constructor
        protected ControlsCoreScene(ScaryCastleGame game, AtlasImage image, float imageScale, float verticalOffset, bool isKeyboard)
            : base(game, "@Menu.Titles.Controls")
        {
            var labelNames = Enum.GetNames<LabelName>();
            labels = new TextSprite[labelNames.Length];

            var platform = EngendroGame.RunningPlatform;

            // Labels
            for (var i = 0; i < labels.Length; i++)
            {
                labels[i] = new TextSprite(game, Fonts.Common)
                {
                    Color = ColorPalette.TextWhite,
                    Scale = ScaleInfo.ControlLabel,
                    Text = isKeyboard ? $"@Menu.Controls.Keyboard.{labelNames[i]}" : $"@Menu.Controls.{platform}.{labelNames[i]}"
                };

                // Override weapon name
                if (isKeyboard)
                {
                    if (i == (int)LabelName.MovementAlt)
                    {
                        labels[i].Text = $"@Menu.Controls.Keyboard.{LabelName.Movement}";
                    }
                }
            }

            // Image
            this.image = new ImageSprite(game, image)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = new Vector2(Screen.Center.X, Screen.Center.Y + verticalOffset),
                Scale = new Vector2(imageScale)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            image.Draw(gameTime);
            for (var i = 0; i < labels.Length; i++)
            {
                if (labels[i].Position != Vector2.Zero)
                {
                    labels[i].Draw(gameTime);
                }
            }
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }

        // SetLabel
        protected void SetLabel(LabelName labelName, float x, float y, RectanglePoint pivotOrigin)
        {
            labels[(int)labelName].Position = new Vector2(x, y);
            labels[(int)labelName].PivotOrigin = pivotOrigin;
        }

        #endregion
    }
}
