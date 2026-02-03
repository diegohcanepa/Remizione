using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Meter
    /// </summary>
    public sealed class Meter : GameObject
    {
        #region Private fields

        private readonly ImageSprite back;
        private readonly ImageSprite container;
        private readonly ImageSprite fore;
        private readonly Vector2 padding = new(.5f);
        private static readonly Color previousValue = new(125, 56, 51);
        private readonly ImageSprite previousValue1;
        private readonly FloatTween tween = new() { StartDelay = 200 };
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
                Color = Color.Black,
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

            if (Alignment == HorizontalAlignment.Center)
            {
                var xOffset = container.BoundingBox.Width / 2;
                container.X -= xOffset;
                back.X -= xOffset;
                fore.X -= xOffset;
                previousValue1.X -= xOffset;
            }

            else if (Alignment == HorizontalAlignment.Right)
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
            if (MaximumValue <= 0)
                return 0;
            else
                return val / MaximumValue * width;
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
            get;
            set
            {
                if (value != field)
                {
                    field = value;
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
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    this.Value = field; // setea al maximo
                    back.ScaleX = width;  // back siempre ancho fijo
                    container.ScaleX = width + (padding.X * 2);
                    Invalidate();
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Invalidate();
                }
            }
        }

        // Value
        public int Value
        {
            get;
            set
            {
                if (value != field)
                {
                    float newWidth = GetScaledWidth(value);

                    if (value < field)
                    {
                        float prevWidth = fore.ScaleX;
                        previousValue1.ScaleX = tween.IsRunning ? tween.CurrentValue : prevWidth;
                        tween.Start(TweenStyle.CubicIn, previousValue1.ScaleX, newWidth, 1000);
                    }
                    else
                    {
                        previousValue1.ScaleX = 0;
                        tween.Stop();
                    }

                    field = value;
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
                    fore.ScaleX = GetScaledWidth(Value);
                    previousValue1.ScaleX = GetScaledWidth(Value);
                    Invalidate();
                }
            }
        }
    }
}
