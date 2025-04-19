using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Meter
    /// </summary>
    public sealed class Meter : GameObject
    {
        private readonly ImageSprite back;
        private readonly ImageSprite container;
        private readonly ImageSprite fore;
        private bool isResetting;
        private int maximumValue;
        private Vector2 position;
        private static readonly Color previousValue = new(207, 117, 43);
        private readonly ImageSprite previousValue1;
        private readonly FloatTween tween = new() { StartDelay = 200 };
        private float value;

        // Constructor
        public Meter(EngendroGame game, Color backColor, Color foreColor)
            : base(game)
        {
            this.BackColor = backColor;
            this.ForeColor = foreColor;

            this.container = new ImageSprite(game, Atlases.UI.Pixel)
            {
                Color = Color.Black,
                Opacity = .6f,
                ScaleY = 2.4f
            };

            this.back = new ImageSprite(game, Atlases.UI.Pixel)
            {
                Color = backColor,
                ScaleY = 1.2f
            };

            this.fore = new ImageSprite(game, Atlases.UI.Pixel)
            {
                Color = foreColor,
                ScaleY = 1.2f
            };

            this.previousValue1 = new ImageSprite(game, Atlases.UI.Pixel)
            {
                Color = previousValue,
                ScaleY = 1.2f
            };
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            container.Position = Position - Vector2.One;
            back.Position = Position - Vector2.One / 2;
            fore.Position = Position - new Vector2(.5f);
            previousValue1.Position = Position - new Vector2(.5f);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            container.Draw(gameTime);
            back.Draw(gameTime);

            if (previousValue1.ScaleX > 0)
                previousValue1.Draw(gameTime);

            fore.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (tween.IsRunning)
            {
                tween.Update(gameTime);
                previousValue1.ScaleX = tween.CurrentValue;
            }
        }

        #endregion

        // BackColor
        public Color BackColor { get; }

        // BoundingBox
        public RectangleF BoundingBox => container.BoundingBox;

        // ForeColor
        public Color ForeColor { get; }

        // MaximumValue
        public int MaximumValue
        {
            get => maximumValue;
            set
            {
                if (value != maximumValue)
                {
                    this.maximumValue = value;
                    back.ScaleX = value;
                    container.ScaleX = value + 1;
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value != position)
                {
                    position = value;
                    Invalidate();
                }
            }
        }

        // Reset
        public void Reset()
        {
            tween.Stop();
            isResetting = true;
            Value = 0;
            isResetting = false;
        }

        // Value
        public float Value
        {
            get => value;
            set
            {
                if (value != this.value)
                {
                    if (value < this.value && !isResetting)
                    {
                        var diff = Math.Abs(fore.ScaleX - value);
                        previousValue1.ScaleX = tween.IsRunning ? tween.CurrentValue : fore.ScaleX;
                        tween.Start(TweenStyle.CubicIn, previousValue1.ScaleX, fore.ScaleX - diff, 1000);
                    }

                    this.value = value;
                    fore.ScaleX = value;
                }
            }
        }
    }
}