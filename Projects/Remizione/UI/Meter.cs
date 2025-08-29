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
        #region Private fields

        private HorizontalAlignment alignment;
        private readonly ImageSprite back;
        private readonly ImageSprite container;
        private readonly ImageSprite fore;
        private int maximumValue;
        private readonly Vector2 padding = new(.5f);
        private Vector2 position;
        private static readonly Color previousValue = new(171, 81, 48);
        private readonly ImageSprite previousValue1;
        private readonly FloatTween tween = new() { StartDelay = 200 };
        private int value;
        private float width;

        #endregion

        // Constructor
        public Meter(EngendroGame game, Color backColor, Color foreColor, Vector2 size)
            : base(game)
        {
            this.BackColor = backColor;
            this.ForeColor = foreColor;
            this.width = size.X;

            // Container
            this.container = new ImageSprite(game, Atlases.UI.Pixel)
            {
                Color = new(41, 29, 43),
                ScaleY = size.Y,
                ScaleX = size.X + (padding.X * 2) // ancho fijo para el container
            };

            // Back
            this.back = new ImageSprite(game, Atlases.UI.Pixel)
            {
                Color = backColor,
                ScaleY = container.ScaleY - (padding.Y * 2),
                ScaleX = size.X
            };

            // Fore
            this.fore = new ImageSprite(game, Atlases.UI.Pixel)
            {
                Color = foreColor,
                ScaleY = container.ScaleY - (padding.Y * 2)
            };

            // Previous value
            this.previousValue1 = new ImageSprite(game, Atlases.UI.Pixel)
            {
                Color = previousValue,
                ScaleY = container.ScaleY - (padding.Y * 2)
            };
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            container.Position = Position;
            back.Position = Position + padding;
            fore.Position = Position + padding;
            previousValue1.Position = Position + padding;

            if (alignment == HorizontalAlignment.Center)
            {
                var xOffset = container.BoundingBox.Width / 2;
                container.X -= xOffset;
                back.X -= xOffset;
                fore.X -= xOffset;
                previousValue1.X -= xOffset;
            }

            else if (alignment == HorizontalAlignment.Right)
            {
                var xOffset = container.BoundingBox.Width;
                container.X += xOffset;
                back.X += xOffset;
                fore.X += xOffset;
                previousValue1.X += xOffset;
            }
        }

        // Convierte un valor lógico (0..MaximumValue) a ancho proporcional (0..fixedWidth)
        private float GetScaledWidth(float val)
        {
            if (maximumValue <= 0) 
                return 0;
            else
                return (val / maximumValue) * width;
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

        // Alignment
        public HorizontalAlignment Alignment
        {
            get => alignment;
            set
            {
                if (value != alignment)
                {
                    this.alignment = value;
                    Invalidate();
                }
            }
        }

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
                    this.Value = maximumValue; // setea al maximo
                    back.ScaleX = width;  // back siempre ancho fijo
                    container.ScaleX = width + (padding.X * 2);
                    Invalidate();
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

        // Value
        public int Value
        {
            get => value;
            set
            {
                if (value != this.value)
                {
                    float newWidth = GetScaledWidth(value);

                    if (value < this.value)
                    {
                        float prevWidth = fore.ScaleX;
                        float diff = Math.Abs(prevWidth - newWidth);
                        previousValue1.ScaleX = tween.IsRunning ? tween.CurrentValue : prevWidth;
                        tween.Start(TweenStyle.CubicIn, previousValue1.ScaleX, newWidth, 1000);
                    }
                    else
                    {
                        previousValue1.ScaleX = 0;
                        tween.Stop();
                    }

                    this.value = value;
                    fore.ScaleX = newWidth;
                }
            }
        }

        // Width
        public float Width
        {
            get => width;
            set
            {
                if (Math.Abs(width - value) > float.Epsilon)
                {
                    width = value;
                    back.ScaleX = width;
                    container.ScaleX = width + (padding.X * 2);
                    fore.ScaleX = GetScaledWidth(this.value);
                    previousValue1.ScaleX = GetScaledWidth(this.value);
                    Invalidate();
                }
            }
        }
    }
}
