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
        private readonly bool isInitializing = true;
        private static readonly AtlasImage[] borderPieces = [
            Atlases.UI.GetImage("DialogBoxBorderLT"),
            Atlases.UI.GetImage("DialogBoxBorderT"),
            Atlases.UI.GetImage("DialogBoxBorderRT"),
            Atlases.UI.GetImage("DialogBoxBorderL"),
            Atlases.UI.GetImage("DialogBoxBorderR"),
            Atlases.UI.GetImage("DialogBoxBorderLB"),
            Atlases.UI.GetImage("DialogBoxBorderB"),
            Atlases.UI.GetImage("DialogBoxBorderRB")
        ];

        private static readonly AtlasImage[] fillPieces = [
            Atlases.UI.GetImage("DialogBoxFillLT"),
            Atlases.UI.GetImage("DialogBoxFillT"),
            Atlases.UI.GetImage("DialogBoxFillRT"),
            Atlases.UI.GetImage("DialogBoxFillL"),
            Atlases.UI.GetImage("DialogBoxFillR"),
            Atlases.UI.GetImage("DialogBoxFillLB"),
            Atlases.UI.GetImage("DialogBoxFillB"),
            Atlases.UI.GetImage("DialogBoxFillRB")
        ];

        private readonly Sprite[] borderSprites = new Sprite[9];
        private readonly Sprite[] fillSprites = new Sprite[9];

        #region Constructor

        // Constructor
        public DynamicWindow()
        {
            int sourceIndex = 0;
            for (int i = 0; i < 9; i++)
            {
                // Not used
                if (i == (int)RectanglePoint.Center)
                    continue;

                borderSprites[i] = new()
                {
                    RenderImage = borderPieces[sourceIndex],
                    PivotOrigin = RectanglePoint.LeftTop
                };

                fillSprites[i] = new()
                {
                    RenderImage = fillPieces[sourceIndex],
                    PivotOrigin = RectanglePoint.LeftTop
                };

                sourceIndex++;
            }

            this.PivotOrigin = RectanglePoint.Center;
            this.Position = Screen.Center;

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
            borderSprites[(int)RectanglePoint.LeftTop].Position = new Vector2(posX, posY);
            borderSprites[(int)RectanglePoint.RightTop].Position = new Vector2(posX + Width - 4, posY);
            borderSprites[(int)RectanglePoint.LeftBottom].Position = new Vector2(posX, posY + Height - 4);
            borderSprites[(int)RectanglePoint.RightBottom].Position = new Vector2(posX + Width - 4, posY + Height - 4);

            // 3. Escalar y Posicionar Bordes Horizontales
            float horizontalScale = (Width - 8) / 4f;

            var top = borderSprites[(int)RectanglePoint.Top];
            top.Position = new Vector2(posX + 4, posY);
            top.ScaleX = horizontalScale;
            top.ScaleY = 1;

            var bottom = borderSprites[(int)RectanglePoint.Bottom];
            bottom.Position = new Vector2(posX + 4, posY + Height - 4);
            bottom.ScaleX = horizontalScale;
            bottom.ScaleY = 1;

            // 4. Escalar y Posicionar Bordes Verticales
            float verticalScale = (Height - 8) / 4f;

            var left = borderSprites[(int)RectanglePoint.Left];
            left.Position = new Vector2(posX, posY + 4);
            left.ScaleX = 1;
            left.ScaleY = verticalScale;

            var right = borderSprites[(int)RectanglePoint.Right];
            right.Position = new Vector2(posX + Width - 4, posY + 4);
            right.ScaleX = 1;
            right.ScaleY = verticalScale;

            for (var i = 0; i < borderSprites.Length; i++)
            {
                if (borderSprites[i] != null)
                {
                    fillSprites[i].Position = borderSprites[i].Position;
                    fillSprites[i].ScaleX = borderSprites[i].ScaleX;
                    fillSprites[i].ScaleY = borderSprites[i].ScaleY;
                }
            }

            // Área total ocupada por el window
            BoundingBox = new(posX, posY, Width, Height);

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
                fillSprites[i]?.Draw(gameTime);
                borderSprites[i]?.Draw(gameTime);
            }

            Game.Shapes.DrawRectangle(InnerBounds, FillColor);
        }

        #endregion

        // BorderColor
        public Color BorderColor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    for (int i = 0; i < 9; i++)
                    {
                        borderSprites[i]?.Color = field;
                    }
                }
            }
        }

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // FillColor
        public Color FillColor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    for (int i = 0; i < 9; i++)
                    {
                        fillSprites[i]?.Color = field;
                    }
                }
            }
        }

        // Height
        public int Height
        {
            get;
            set
            {
                int validatedValue = value < 8 ? 8 : 8 + ((value - 8 + 3) / 4 * 4);

                if (validatedValue != field)
                {
                    field = validatedValue;
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
                int validatedValue = value < 8 ? 8 : 8 + ((value - 8 + 3) / 4 * 4);

                if (validatedValue != field)
                {
                    field = validatedValue;
                    Layout();
                }
            }
        }
    }
}