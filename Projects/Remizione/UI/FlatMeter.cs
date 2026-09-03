using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// FlatMeter
    /// </summary>
    public sealed class FlatMeter : GameObject
    {
        #region Private fields

        private readonly Sprite back;
        private readonly Sprite container;
        private readonly Sprite diff;
        private readonly Sprite fore;
        private readonly TextSprite amountText;
        private readonly FloatTween tween = new() { StartDelay = 200 };
        private float width;

        #endregion

        // Constructor
        public FlatMeter(Color backColor, Color foreColor, Color diffColor, Vector2 size, float borderSize)
        {
            this.BorderSize = new(borderSize);
            this.width = size.X;

            // Container
            this.container = new Sprite(Atlases.UI.Pixel)
            {
                Color = Color.Black,
                ScaleY = size.Y,
                ScaleX = size.X + (BorderSize.X * 2) // ancho fijo para el container
            };

            // Back
            this.back = new Sprite(Atlases.UI.Pixel)
            {
                Color = backColor,
                ScaleY = container.ScaleY - (BorderSize.Y * 2),
                ScaleX = size.X
            };

            // Fore
            this.fore = new Sprite(Atlases.UI.Pixel)
            {
                Color = foreColor,
                ScaleY = container.ScaleY - (BorderSize.Y * 2)
            };

            // Previous value
            this.diff = new Sprite(Atlases.UI.Pixel)
            {
                Color = diffColor,
                ScaleY = container.ScaleY - (BorderSize.Y * 2)
            };

            // AmountText
            this.amountText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.MouseCursor,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Medium
            };
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            container.Position = Position;
            back.Position = Position + BorderSize;
            fore.Position = Position + BorderSize;
            diff.Position = Position + BorderSize;

            var xOffset = container.BoundingBox.Width / 2;
            container.X -= xOffset;
            back.X -= xOffset;
            fore.X -= xOffset;
            diff.X -= xOffset;

            amountText.Text = Value.ToString(CultureInfo.InvariantCulture);
            amountText.Position = container.BoundingBox.GetPoint(RectanglePoint.Top, 0, 1);
        }

        // Convierte un valor lógico (0..MaximumValue) a ancho proporcional (0..fixedWidth)
        private float GetScaledWidth(float val)
        {
            return MaximumValue <= 0 ? 0 : val / MaximumValue * width;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            container.Draw(gameTime);
            back.Draw(gameTime);
            if (diff.ScaleX > 0)
                diff.Draw(gameTime);
            fore.Draw(gameTime);

            if (ShowAmount)
                amountText.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (tween.IsRunning)
            {
                tween.Update(gameTime);
                diff.ScaleX = tween.CurrentValue;
            }
        }

        #endregion

        // BackColor
        public Color BackColor
        {
            get => back.Color;
            set => back.Color = value;
        }

        // BoundingBox
        public RectangleF BoundingBox => container.BoundingBox;

        // BorderSize
        public Vector2 BorderSize { get; }

        // CreateHPMeter
        public static FlatMeter CreateHPMeter()
        {
            return new(ColorPalette.HPMeter.Back, ColorPalette.HPMeter.Fore, ColorPalette.HPMeter.Diff, new(10, 2.5f), .5f)
            {
                ShowAmount = true
            };
        }

        // DiffColor
        public Color DiffColor
        {
            get => diff.Color;
            set => diff.Color = value;
        }

        // ForeColor
        public Color ForeColor
        {
            get => fore.Color;
            set => fore.Color = value;
        }

        // IsAnimating
        public bool IsAnimating => tween.IsRunning;

        // MaximumValue
        public float MaximumValue
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    this.Value = field; // setea al maximo
                    back.ScaleX = width;  // back siempre ancho fijo
                    container.ScaleX = width + (BorderSize.X * 2);
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

        // Ratio
        public float Ratio => Value / MaximumValue;

        // ShowAmount
        public bool ShowAmount { get; set; }

        // Value
        public float Value
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
                        diff.ScaleX = tween.IsRunning ? tween.CurrentValue : prevWidth;
                        tween.Start(TweenStyle.CubicIn, diff.ScaleX, newWidth, 1000);
                    }
                    else
                    {
                        diff.ScaleX = 0;
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
                    container.ScaleX = width + (BorderSize.X * 2);
                    fore.ScaleX = GetScaledWidth(Value);
                    diff.ScaleX = GetScaledWidth(Value);
                    Invalidate();
                }
            }
        }
    }
}
