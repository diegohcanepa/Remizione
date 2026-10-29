using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UITextButton
    /// </summary>
    public sealed class UITextButton : UIButton
    {
        #region Private members

        private RectangleF boundingBox;
        private readonly Color disabledColor = Color.Gray;
        private readonly TextSprite label;
        private readonly Sprite[] strips;

        #endregion

        #region Constructor

        // Constructor
        public UITextButton()
        {
            strips = new Sprite[3];

            strips[0] = new(Atlases.UI.UIButtonContainerEdge) { PivotOrigin = RectanglePoint.RightTop };
            strips[1] = new(Atlases.UI.UIButtonContainerPattern) { PivotOrigin = RectanglePoint.Center };
            strips[2] = new(Atlases.UI.UIButtonContainerEdge)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Effects = SpriteEffects.FlipHorizontally
            };

            this.label = new TextSprite(Fonts.CommonOutline)
            {
                Color = Colors.Palette.Text,
            };

            this.Scale = ScaleInfo.Text.Huge;
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (label.IsEmpty)
                boundingBox = RectangleF.Empty;

            strips[1].ScaleX = float.Max(label.BoundingBox.Width, MinimumWidth - strips[0].BoundingBox.Width - strips[2].BoundingBox.Width) / 2;

            strips[1].Position = label.BoundingBox.GetPoint(RectanglePoint.Center, 0, -.75f);

            strips[0].Position = strips[1].BoundingBox.GetPoint(RectanglePoint.LeftTop);
            strips[2].Position = strips[1].BoundingBox.GetPoint(RectanglePoint.RightTop);

            boundingBox = RectangleF.Union(strips[0].BoundingBox, strips[1].BoundingBox, strips[2].BoundingBox);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!label.IsEmpty)
            {
                strips[0].Draw(gameTime);
                strips[1].Draw(gameTime);
                strips[2].Draw(gameTime);
                label.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            label.Update(gameTime);

            if (IsMouseOver && IsEnabled)
            {
                label.Color = HoverColor;
            }
            else
            {
                label.Color = IsEnabled ? TextColor : disabledColor;
                label.Scale = Scale;
            }
        }

        #endregion

        // BoundingBox
        public override RectangleF BoundingBox => boundingBox;

        // Font
        public Font Font
        {
            get => label.Font;
            set => label.Font = value;
        }

        // HoverColor
        public Color HoverColor { get; set; } = Colors.Palette.TerraLight;

        // MinimumWidth
        public float MinimumWidth
        {
            get;
            set
            {
                if (value < 0)
                    value = 0;

                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        }

        // PivotOrigin
        public override RectanglePoint PivotOrigin
        {
            get => label.PivotOrigin;
            set
            {
                if (value != label.PivotOrigin)
                {
                    label.PivotOrigin = value;
                    Refresh();
                }
            }
        }

        // Position
        public override Vector2 Position
        {
            get => label.Position;
            set
            {
                if (value != label.Position)
                {
                    label.Position = value;
                    Refresh();
                }
            }
        }

        // Scale
        public override Vector2 Scale
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    label.Scale = field;
                    Refresh();
                }
            }
        }

        // Text
        public string? Text
        {
            get => label.Text;
            set
            {
                if (value != label.Text)
                {
                    label.Text = value;
                    Refresh();
                }
            }
        }

        // TextColor
        public Color TextColor { get; set; } = Colors.Palette.Text;

        // X
        public override float X
        {
            get => label.X;
            set
            {
                if (value != label.X)
                {
                    label.X = value;
                    Refresh();
                }
            }
        }

        // Y
        public override float Y
        {
            get => label.Y;
            set
            {
                if (value != label.Y)
                {
                    label.Y = value;
                    Refresh();
                }
            }
        }
    }
}
