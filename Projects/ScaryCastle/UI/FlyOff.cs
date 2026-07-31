using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// FlyOff
    /// </summary>
    public sealed class FlyOff : SessionGameObject<GameSession>
    {
        private const int fadeDuration = 250;
        private static readonly float defaultScale = ScaleInfo.Text.VeryLarge.X;

        private Sprite? activeSprite;
        private readonly Sprite icon = new() { PivotOrigin = RectanglePoint.Bottom, Scale = ScaleInfo.UIElement.Medium };
        private readonly FloatTween opacityTween = new();
        private readonly TextSprite text = new(Fonts.CommonOutline) { PivotOrigin = RectanglePoint.Bottom };
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        // Constructor
        public FlyOff(GameSession session)
            : base(session)
        {
        }

        #region Private members

        // Launch
        private void Launch(Vector2 origin, Sprite sprite, Vector2 distance, int duration)
        {
            activeSprite = sprite;

            if (duration < fadeDuration)
                duration = fadeDuration;

            sprite.Position = origin;

            if (distance.Y != 0)
            {
                yTween.Start(TweenStyle.CubicOut, origin.Y, origin.Y + distance.Y, duration);
                sprite.Tweens.YTween = yTween;
            }

            if (distance.X != 0)
            {
                xTween.Start(TweenStyle.CubicOut, origin.X, origin.X + distance.X, duration);
                sprite.Tweens.XTween = xTween;
            }

            opacityTween.StartDelay = duration - fadeDuration;
            opacityTween.Start(TweenStyle.CubicIn, 1, 0, fadeDuration);
            sprite.Tweens.OpacityTween = opacityTween;
        }

        // ShowTextCore
        private void ShowTextCore(Vector2 origin, string value, Color color, Vector2 distance, float scale, int duration)
        {
            text.Color = color;
            text.Scale = new(scale);
            text.Text = value;
            Launch(origin, text, distance, duration);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            activeSprite?.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            activeSprite?.Update(gameTime);

            if (!IsVisible)
                Session.ObjectPools.FlyOffs.Return(this);
        }

        #endregion

        // IsVisible
        public bool IsVisible => xTween.IsRunning || yTween.IsRunning || opacityTween.IsRunning;

        // ShowIcon
        public void ShowIcon(Vector2 origin, AtlasImage image, int duration = 2000)
        {
            icon.RenderImage = image;
            Launch(origin, icon, new Vector2(0, -3), duration);
        }

        // ShowText
        public void ShowText(Vector2 origin, string value, Color color, int duration = 1000)
        {
            ShowText(origin, value, color, defaultScale, duration);
        }

        // ShowText
        public void ShowText(Vector2 origin, string value, Color color, float scale, int duration = 1000)
        {
            ShowTextCore(origin, value, color, new Vector2(0, -6), scale, duration);
        }

        // ShowAmount
        public void ShowAmount(GameThing source, Color color, int amount)
        {
            if (amount == 0)
                return;

            var origin = source.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Top, 0, -4);
            var deltaX = Random.Shared.Next(3, 6);
            var horzDirection = source.Direction == FacingDirection.Left ? deltaX : -deltaX;

            ShowTextCore(origin, amount.ToString(CultureInfo.InvariantCulture), color, new(horzDirection, -10), ScaleInfo.Text.Huge.X, 1700);
        }
    }
}
