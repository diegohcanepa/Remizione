using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remizione.UI
{
    /// <summary>
    /// GameObject
    /// </summary>
    public sealed class UIRoomTitle : GameObject
    {
        private readonly TextSprite labelText;
        private readonly FloatTween opacityTween = new();
        private readonly Vector2Tween scaleTween = new();

        // Constructor
        public UIRoomTitle()
        {
            this.labelText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.MouseCursor.Tooltip,
                PivotOrigin = RectanglePoint.Top
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearWrap);
            labelText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            labelText.Update(gameTime);
        }

        #endregion

        // Show
        public void Show(Room room)
        {
            void Hide()
            {
                opacityTween.StartDelay = 1000;
                opacityTween.Start(TweenStyle.CubicIn, labelText.Opacity, 0, 2000);
                labelText.Tweens.OpacityTween = opacityTween;
            }

            labelText.Text = TextRepository.GetValue($"Room.{room.Name}");

            labelText.Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 14);

            labelText.Scale = new(.16f);
            scaleTween.Start(TweenStyle.CubicIn, labelText.Scale, labelText.Scale * 1.1f, 8000);
            labelText.Tweens.ScaleTween = scaleTween;

            opacityTween.StartDelay = 0;
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 4000, Hide);
            labelText.Tweens.OpacityTween = opacityTween;
        }
    }
}
