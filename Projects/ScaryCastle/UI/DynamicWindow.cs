using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// DynamicWindow
    /// </summary>
    public class DynamicWindow : GameObject
    {
        private readonly Sprite[] sprites = new Sprite[9];
        private bool isInitializing = true;

        #region Constructor

        // Constructor
        public DynamicWindow(Vector2 position, int width, int height, RectanglePoint pivotOrigin, AtlasImage[] windowPieces)
        {
            if (windowPieces.Length < 8)
                throw new ArgumentException("The window container requires exactly 8 images.", nameof(windowPieces));

            int sourceIndex = 0;
            for (int i = 0; i < 9; i++)
            {
                // Not used
                if (i == (int)RectanglePoint.Center)
                    continue;

                sprites[i] = new()
                {
                    RenderImage = windowPieces[sourceIndex++],
                    PivotOrigin = RectanglePoint.LeftTop
                };
            }

            this.Width = width;
            this.Height = height;
            this.PivotOrigin = pivotOrigin;
            this.Position = position;

            isInitializing = false;
            Layout();
        }

        #endregion

        #region Private members

        // Layout
        private void Layout()
        {
            if (isInitializing)
                return;

            // 1. Calcular el origen real (TopLeft) de la ventana aplicando el pivote
            Vector2 renderOrigin = Position;

            switch (PivotOrigin)
            {
                case RectanglePoint.LeftTop:
                    break;
                
                case RectanglePoint.Top:
                    renderOrigin.X -= Width / 2f;
                    break;
                
                case RectanglePoint.RightTop:
                    renderOrigin.X -= Width;
                    break;
                
                case RectanglePoint.Left:
                    renderOrigin.Y -= Height / 2f;
                    break;
                
                case RectanglePoint.Center:
                    renderOrigin.X -= Width / 2f;
                    renderOrigin.Y -= Height / 2f;
                    break;
                
                case RectanglePoint.Right:
                    renderOrigin.X -= Width;
                    renderOrigin.Y -= Height / 2f;
                    break;
                
                case RectanglePoint.LeftBottom:
                    renderOrigin.Y -= Height;
                    break;
                
                case RectanglePoint.Bottom:
                    renderOrigin.X -= Width / 2f;
                    renderOrigin.Y -= Height;
                    break;
                
                case RectanglePoint.RightBottom:
                    renderOrigin.X -= Width;
                    renderOrigin.Y -= Height;
                    break;
            }

            float posX = MathF.Round(renderOrigin.X);
            float posY = MathF.Round(renderOrigin.Y);

            // 2. Posicionar Esquinas basadas en el origen corregido
            sprites[(int)RectanglePoint.LeftTop].Position = new Vector2(posX, posY);
            sprites[(int)RectanglePoint.RightTop].Position = new Vector2(posX + Width - 4, posY);
            sprites[(int)RectanglePoint.LeftBottom].Position = new Vector2(posX, posY + Height - 4);
            sprites[(int)RectanglePoint.RightBottom].Position = new Vector2(posX + Width - 4, posY + Height - 4);

            // 3. Escalar y Posicionar Bordes Horizontales
            float horizontalScale = (Width - 8) / 4f;

            var top = sprites[(int)RectanglePoint.Top];
            top.Position = new Vector2(posX + 4, posY);
            top.ScaleX = horizontalScale;
            top.ScaleY = 1f;

            var bottom = sprites[(int)RectanglePoint.Bottom];
            bottom.Position = new Vector2(posX + 4, posY + Height - 4);
            bottom.ScaleX = horizontalScale;
            bottom.ScaleY = 1f;

            // 4. Escalar y Posicionar Bordes Verticales
            float verticalScale = (Height - 8) / 4f;

            var left = sprites[(int)RectanglePoint.Left];
            left.Position = new Vector2(posX, posY + 4);
            left.ScaleX = 1f;
            left.ScaleY = verticalScale;

            var right = sprites[(int)RectanglePoint.Right];
            right.Position = new Vector2(posX + Width - 4, posY + 4);
            right.ScaleX = 1f;
            right.ScaleY = verticalScale;

            // El área interna útil también se desplaza con el pivote de forma automática
            InnerBounds = new(posX + 4, posY + 4, Width - 8, Height - 8);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (int i = 0; i < 9; i++)
            {
                sprites[i]?.Draw(gameTime);
            }
        }

        #endregion

        // Height
        public int Height
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Layout();
                }
            }
        }

        // InnerBounds
        public RectangleF InnerBounds { get; private set; }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Layout();
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get;
            set
            {
                if (MathF.Round(value.X) != MathF.Round(field.X) ||
                    MathF.Round(value.Y) != MathF.Round(field.Y))
                {
                    field = value;
                    Layout();
                }
                else
                {
                    field = value;
                }
            }
        }

        // Width
        public int Width
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Layout();
                }
            }
        }
    }
}