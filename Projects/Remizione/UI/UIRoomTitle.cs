using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
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

        // Constructor
        public UIRoomTitle()
        {
            this.labelText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.MouseCursor.Tooltip,
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 14),
                Scale = ScaleInfo.Text.ExtraGiant
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            labelText.Draw(gameTime);
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
            opacityTween.StartDelay = 0;
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 4000, Hide);
            labelText.Tweens.OpacityTween = opacityTween;
        }
    }
}
