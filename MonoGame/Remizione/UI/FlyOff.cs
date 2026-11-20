using System;
using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// FlyOff
    /// </summary>
    public sealed class FlyOff : SessionGameObject<GameSession>
    {
        private const int fadeDuration = 100;
        private const int fadeOutMaxDuration = 900;
        private const int fadeOutMinDuration = 420;
        private const int scaleInMaxDuration = 280;
        private const int scaleInMinDuration = 100;

        private static readonly float defaultScale = ScaleInfo.Text.Small.X;
        private static readonly Vector2[] LaunchOffsets =
        [
            new(-5f, -5f), // Izquierda, subida media
            new( 5f, -8f), // Derecha, subida máxima
            new(-7f, -2f), // Izquierda apertura, subida corta
            new( 7f, -5f), // Derecha apertura, subida media
            new(-1f, -8f), // Centro-izquierda, subida máxima
            new( 1f, -2f)  // Centro-derecha, subida corta
        ];

        private Sprite? activeSprite;
        private readonly Sprite icon = new() { PivotOrigin = RectanglePoint.Bottom, Scale = ScaleInfo.UIElement.Medium };
        private readonly FloatTween opacityTween = new();
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleInTween = new();
        private bool shake;
        private readonly FloatTween shakeTween = FloatTween.Create(TweenStyle.Linear, 0, .5f, 50, -1);
        private static int spawnIndex;
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
        private void Launch(Vector2 origin, Sprite sprite, int duration)
        {
            var isIcon = sprite is not TextSprite;
            var distance = isIcon ? LaunchOffsets[spawnIndex] : new(0, -1);
            spawnIndex = (spawnIndex + 1) % LaunchOffsets.Length;

            activeSprite = sprite;
            shake = isIcon;

            var currentFadeDuration = isIcon
                ? Random.Shared.Next(fadeOutMinDuration, fadeOutMaxDuration + 1)
                : fadeDuration;

            if (duration < currentFadeDuration)
                duration = currentFadeDuration;

            var targetScale = sprite.Scale;
            var scaleInDuration = Random.Shared.Next(scaleInMinDuration, scaleInMaxDuration + 1);

            sprite.Position = origin;
            sprite.Opacity = 1f;

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

            scaleInTween.Start(TweenStyle.Linear, Vector2.Zero, targetScale, scaleInDuration);
            sprite.Tweens.ScaleTween = scaleInTween;

            opacityTween.StartDelay = duration - currentFadeDuration;
            opacityTween.Start(TweenStyle.QuadraticIn, 1, 0, currentFadeDuration);
            sprite.Tweens.OpacityTween = opacityTween;

            if (shake)
            {
                rotationTween.Start(TweenStyle.QuadraticInOut, 0, 5, 50, duration / 50 / 2);
                sprite.Tweens.RotationTween = rotationTween;
            }
        }

        // ShowTextCore
        private void ShowTextCore(Vector2 origin, string value, Color color, float scale, int duration)
        {
            text.Color = color;
            text.Scale = new(scale);
            text.Text = value;
            Launch(origin, text, duration);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (activeSprite != null)
            {
                Game.SpriteBatch.Begin(Session.Camera, activeSprite is TextSprite ? SamplerState.LinearClamp : SamplerState.PointClamp);

                if (shake)
                    activeSprite.X += shakeTween.CurrentValue;

                activeSprite.Draw(gameTime);

                if (shake)
                    activeSprite.X -= shakeTween.CurrentValue;

                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            shakeTween.Update(gameTime);
            activeSprite?.Update(gameTime);

            if (!IsVisible)
                Session.ObjectPools.FlyOffs.Return(this);
        }

        #endregion

        // IsVisible
        public bool IsVisible => xTween.IsRunning || yTween.IsRunning || opacityTween.IsRunning;

        // ShowIcon
        public void ShowIcon(Vector2 origin, AtlasImage image, float scale = 1)
        {
            icon.RenderImage = image;
            icon.Scale = new(scale);
            Launch(origin, icon, 1800);
        }

        // ShowText
        public void ShowText(Vector2 origin, string value, Color color, int duration)
        {
            ShowText(origin, value, color, defaultScale, duration);
        }

        // ShowText
        public void ShowText(Vector2 origin, string value, Color color, float scale, int duration)
        {
            ShowTextCore(origin, value, color, scale, duration);
        }
    }
}