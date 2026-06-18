using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// UIGooMeter
    /// </summary>
    public sealed class UIGooMeter : GameObject
    {
        #region Private fields

        private readonly Sprite container;
        private int currentDisplayedIndex;
        private const float fillSpeed = 5f;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private float visualValue;

        #endregion

        #region Constructor

        // Constructor
        public UIGooMeter()
        {
            container = new(Atlases.UI.GooMeter[0])
            {
                Position = Screen.Area.GetPoint(RectanglePoint.LeftTop, 6, 2)
            };

            currentDisplayedIndex = 0;
        }

        #endregion

        #region Private members

        // AnimateJuice
        private void AnimateJuice()
        {
            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 3, 50, 6);
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.1f, 150, 2);

            container.Tweens.RotationTween = rotationTween;
            container.Tweens.ScaleTween = scaleTween;
        }

        // UpdateSpriteIndex
        private void UpdateSpriteIndex(int newIndex)
        {
            // Asegurar que no nos salgamos del rango de texturas disponibles
            int clampedIndex = MathHelper.Clamp(newIndex, 0, Atlases.UI.GooMeter.Count - 1);

            if (currentDisplayedIndex != clampedIndex)
            {
                currentDisplayedIndex = clampedIndex;
                container.RenderImage = Atlases.UI.GooMeter[currentDisplayedIndex];
                AnimateJuice();
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null) return;

            Game.SpriteBatch.Begin(Game.Camera);
            container.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor == null)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            container.Update(gameTime);

            float targetValue = Actor.Energy;

            // Comparación directa (segura acá por el Clamping inferior) 
            // o podés usar: if (Math.Abs(visualValue - targetValue) > 0.001f)
            if (visualValue != targetValue)
            {
                if (visualValue < targetValue)
                {
                    visualValue = Math.Min(visualValue + (fillSpeed * dt), targetValue);
                }
                else
                {
                    visualValue = Math.Max(visualValue - (fillSpeed * dt), targetValue);
                }

                int targetIndex = (int)Math.Round(visualValue);
                UpdateSpriteIndex(targetIndex);
            }
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field != null)
                    {
                        visualValue = field.Energy;
                        UpdateSpriteIndex(field.Energy);
                    }
                }
            }
        }
    }
}